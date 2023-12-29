namespace Lavalink4NET.DiscordNet;

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Lavalink4NET.Clients;
using Lavalink4NET.Clients.Events;
using Lavalink4NET.Events;

/// <summary>
///     A wrapper for the discord client from the "Discord.NET" discord client library. (https://github.com/discord-net/Discord.Net)
/// </summary>
public sealed class DiscordClientWrapper : IDiscordClientWrapper, IDisposable
{
    private readonly BaseSocketClient _baseSocketClient;
    private readonly TaskCompletionSource<ClientInformation> _readyTaskCompletionSource;
    private readonly int? _shardCount;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DiscordClientWrapper"/> class.
    /// </summary>
    /// <param name="client">the sharded discord client</param>
    public DiscordClientWrapper(DiscordShardedClient client)
        : this(client as BaseSocketClient)
    {
        // _shardCount is null here, and is retrieved dynamically from the client.
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DiscordClientWrapper"/> class.
    /// </summary>
    /// <param name="client">the sharded discord client</param>
    /// <param name="shards">the number of total shards</param>
    public DiscordClientWrapper(DiscordShardedClient client, int shards)
        : this(client as BaseSocketClient, shards)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DiscordClientWrapper"/> class.
    /// </summary>
    /// <param name="client">the sharded discord client</param>
    public DiscordClientWrapper(DiscordSocketClient client)
        : this(client, 1)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DiscordClientWrapper"/> class.
    /// </summary>
    /// <param name="baseSocketClient">the discord client</param>
    /// <param name="shards">the number of shards</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="baseSocketClient"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     thrown if the specified shard count is less than 1.
    /// </exception>
    public DiscordClientWrapper(BaseSocketClient baseSocketClient, int shards)
        : this(baseSocketClient)
    {
        if (shards < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(shards), shards, "Shard count must be at least 1.");
        }

        _shardCount = shards;
    }

    private DiscordClientWrapper(BaseSocketClient baseSocketClient)
    {
        ArgumentNullException.ThrowIfNull(baseSocketClient);

        _baseSocketClient = baseSocketClient;
        _baseSocketClient.VoiceServerUpdated += OnVoiceServerUpdated;
        _baseSocketClient.UserVoiceStateUpdated += OnVoiceStateUpdated;

        _readyTaskCompletionSource = new TaskCompletionSource<ClientInformation>(TaskCreationOptions.RunContinuationsAsynchronously);

        if (baseSocketClient is DiscordShardedClient discordShardedClient)
        {
            discordShardedClient.ShardReady += OnShardReady;
        }

        if (baseSocketClient is DiscordSocketClient discordSocketClient)
        {
            discordSocketClient.Ready += OnClientReady;
        }
    }

    /// <summary>
    ///     An asynchronous event which is triggered when the voice server was updated.
    /// </summary>
    public event AsyncEventHandler<VoiceServerUpdatedEventArgs>? VoiceServerUpdated;

    /// <summary>
    ///     An asynchronous event which is triggered when a user voice state was updated.
    /// </summary>
    public event AsyncEventHandler<VoiceStateUpdatedEventArgs>? VoiceStateUpdated;

    /// <summary>
    ///     Disposes the wrapper and unregisters all events attached to the discord client.
    /// </summary>
    public void Dispose()
    {
        _baseSocketClient.VoiceServerUpdated -= OnVoiceServerUpdated;
        _baseSocketClient.UserVoiceStateUpdated -= OnVoiceStateUpdated;

        if (_baseSocketClient is DiscordShardedClient discordShardedClient)
        {
            discordShardedClient.ShardReady -= OnShardReady;
        }

        if (_baseSocketClient is DiscordSocketClient discordSocketClient)
        {
            discordSocketClient.Ready -= OnClientReady;
        }
    }

    /// <summary>
    ///     Gets the snowflake identifier values of the users in the voice channel specified by
    ///     <paramref name="voiceChannelId"/> (the snowflake identifier of the voice channel).
    /// </summary>
    /// <param name="guildId">the guild identifier snowflake where the channel is in</param>
    /// <param name="voiceChannelId">the snowflake identifier of the voice channel</param>
    /// <returns>
    ///     a task that represents the asynchronous operation
    ///     <para>the snowflake identifier values of the users in the voice channel</para>
    /// </returns>
    public ValueTask<ImmutableArray<ulong>> GetChannelUsersAsync(
        ulong guildId,
        ulong voiceChannelId,
        bool includeBots = false,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var guild = _baseSocketClient.GetGuild(guildId);

        if (guild is null)
        {
            // It may be that the guild has been deleted while there was a player for it, return no users
            return ValueTask.FromResult(ImmutableArray<ulong>.Empty);
        }

        var usersEnumerable = guild.Users.Where(x => x.VoiceChannel?.Id == voiceChannelId);

        if (includeBots)
        {
            usersEnumerable = usersEnumerable.Where(x => x.Id != _baseSocketClient.CurrentUser.Id);
        }
        else
        {
            usersEnumerable = usersEnumerable.Where(x => !x.IsBot);
        }

        var users = usersEnumerable
            .Select(s => s.Id)
            .ToImmutableArray();

        return ValueTask.FromResult(users);
    }

    /// <summary>
    ///     Sends a voice channel state update asynchronously.
    /// </summary>
    /// <param name="guildId">the guild snowflake identifier</param>
    /// <param name="voiceChannelId">
    ///     the snowflake identifier of the voice channel to join (if <see langword="null"/> the
    ///     client should disconnect from the voice channel).
    /// </param>
    /// <param name="selfDeaf">a value indicating whether the bot user should be self deafened</param>
    /// <param name="selfMute">a value indicating whether the bot user should be self muted</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    public async ValueTask SendVoiceUpdateAsync(ulong guildId, ulong? voiceChannelId, bool selfDeaf = false, bool selfMute = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var guild = _baseSocketClient.GetGuild(guildId)
            ?? throw new ArgumentException($"Invalid or inaccessible guild: {guildId}", nameof(guildId));

        if (voiceChannelId.HasValue)
        {
            var channel = guild.GetVoiceChannel(voiceChannelId.Value)
                ?? throw new ArgumentException($"Invalid or inaccessible voice channel: {voiceChannelId}", nameof(voiceChannelId));

            await channel
                .ConnectAsync(selfDeaf, selfMute, external: true)
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            return;
        }

        // Disconnect from voice channel
        // Note: Internally it does not matter which voice channel to disconnect from
        await guild
            .VoiceChannels.First()
            .DisconnectAsync()
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask<ClientInformation> WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // If the client was preinitialized, the ready event may have already been fired
        // but the task completion source was not set yet.
        if (!_readyTaskCompletionSource.Task.IsCompleted && _baseSocketClient.CurrentUser is not null)
        {
            var clientInformation = new ClientInformation(
                Label: "discord.net",
                CurrentUserId: _baseSocketClient.CurrentUser.Id,
                ShardCount: GetShardCount());

            _readyTaskCompletionSource.TrySetResult(clientInformation);

            return new(clientInformation);
        }

        return new(_readyTaskCompletionSource.Task.WaitAsync(cancellationToken));
    }

    private int GetShardCount()
    {
        if (_shardCount.HasValue)
        {
            // shard count was given in constructor, or no sharding is used (-> 1)
            return _shardCount.Value;
        }

        // retrieve shard count from client
        Debug.Assert(_baseSocketClient is DiscordShardedClient);
        return ((DiscordShardedClient)_baseSocketClient).Shards.Count;
    }

    private Task OnClientReady()
    {
        var clientInformation = new ClientInformation(
            Label: "discord.net",
            CurrentUserId: _baseSocketClient.CurrentUser.Id,
            ShardCount: GetShardCount());

        _readyTaskCompletionSource.TrySetResult(clientInformation);

        return Task.CompletedTask;
    }

    private Task OnShardReady(DiscordSocketClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        return OnClientReady();
    }

    private Task OnVoiceServerUpdated(SocketVoiceServer voiceServer)
    {
        ArgumentNullException.ThrowIfNull(voiceServer);

        var server = new VoiceServer(
            Token: voiceServer.Token,
            Endpoint: voiceServer.Endpoint);

        var eventArgs = new VoiceServerUpdatedEventArgs(
            guildId: voiceServer.Guild.Id,
            voiceServer: server);

        return VoiceServerUpdated.InvokeAsync(this, eventArgs).AsTask();
    }

    private Task OnVoiceStateUpdated(SocketUser user, SocketVoiceState oldSocketVoiceState, SocketVoiceState socketVoiceState)
    {
        ArgumentNullException.ThrowIfNull(user);

        var guildId = oldSocketVoiceState.VoiceChannel?.Guild?.Id ?? socketVoiceState.VoiceChannel.Guild.Id;

        // create voice state
        var voiceState = new VoiceState(
            VoiceChannelId: socketVoiceState.VoiceChannel?.Id,
            SessionId: socketVoiceState.VoiceSessionId);

        var oldVoiceState = new VoiceState(
            VoiceChannelId: oldSocketVoiceState.VoiceChannel?.Id,
            SessionId: oldSocketVoiceState.VoiceSessionId);

        var eventArgs = new VoiceStateUpdatedEventArgs(
            guildId: guildId,
            userId: user.Id,
            isCurrentUser: user.Id == _baseSocketClient.CurrentUser.Id,
            voiceState: voiceState,
            oldVoiceState: oldVoiceState);

        // invoke event
        return VoiceStateUpdated.InvokeAsync(this, eventArgs).AsTask();
    }
}
