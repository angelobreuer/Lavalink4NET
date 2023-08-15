namespace Lavalink4NET.Players;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Clients.Events;
using Lavalink4NET.Players.Preconditions;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class PlayerManager : IPlayerManager, IDisposable
{
    private readonly ConcurrentDictionary<ulong, ILavalinkPlayerHandle> _handles;
    private readonly ILogger<PlayerManager> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly PlayerContext _playerContext;
    private bool _disposed;

    public PlayerManager(
        IServiceProvider? serviceProvider,
        IDiscordClientWrapper discordClient,
        ILavalinkSessionProvider sessionProvider,
        ISystemClock systemClock,
        ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(sessionProvider);
        ArgumentNullException.ThrowIfNull(systemClock);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        _handles = new ConcurrentDictionary<ulong, ILavalinkPlayerHandle>();

        DiscordClient = discordClient;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<PlayerManager>();

        _playerContext = new PlayerContext(
            ServiceProvider: serviceProvider,
            SessionProvider: sessionProvider,
            DiscordClient: discordClient,
            SystemClock: systemClock);

        DiscordClient.VoiceStateUpdated += OnVoiceStateUpdated;
        DiscordClient.VoiceServerUpdated += OnVoiceServerUpdated;
    }

    public IEnumerable<ILavalinkPlayer> Players
    {
        get
        {
            return _handles.Values
                .Select(x => x.Player)
                .Where(x => x is not null && x.State is not PlayerState.Destroyed)!;
        }
    }

    public IDiscordClientWrapper DiscordClient { get; }

    public async ValueTask<T?> GetPlayerAsync<T>(ulong guildId, CancellationToken cancellationToken = default) where T : class, ILavalinkPlayer
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await GetPlayerAsync(guildId, cancellationToken) as T;
    }

    public async ValueTask<ILavalinkPlayer?> GetPlayerAsync(ulong guildId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_handles.TryGetValue(guildId, out var handle) ||
            handle.Player is null ||
            handle.Player.State is PlayerState.Destroyed)
        {
            return null;
        }

        return await handle
            .GetPlayerAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public IEnumerable<T> GetPlayers<T>() where T : ILavalinkPlayer
    {
        return Players.OfType<T>();
    }

    public bool HasPlayer(ulong guildId)
    {
        return _handles.TryGetValue(guildId, out var handle)
            && handle is { Player.State: not PlayerState.Destroyed };
    }

    public async ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(ulong guildId, ulong voiceChannelId, PlayerFactory<TPlayer, TOptions> playerFactory, IOptions<TOptions> options, CancellationToken cancellationToken = default)
        where TPlayer : ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions
    {
        cancellationToken.ThrowIfCancellationRequested();

        LavalinkPlayerHandle<TPlayer, TOptions> Create(ulong guildId)
        {
            return new LavalinkPlayerHandle<TPlayer, TOptions>(
                guildId: guildId,
                playerContext: _playerContext,
                playerFactory: playerFactory,
                options: options,
                logger: _loggerFactory.CreateLogger<TPlayer>());
        }

        var handle = _handles.GetOrAdd(guildId, Create);

        var selfDeaf = options.Value.SelfDeaf;
        var selfMute = options.Value.SelfMute;

        await DiscordClient
            .SendVoiceUpdateAsync(guildId, voiceChannelId, selfDeaf: selfDeaf, selfMute: selfMute, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var player = await handle
            .GetPlayerAsync(cancellationToken)
            .ConfigureAwait(false);

        if (player is not TPlayer playerValue)
        {
            throw new InvalidOperationException($"Player mismatch. The requested type is {typeof(TPlayer)}, but the current player instance is of type {player.GetType()}");
        }

        return playerValue;
    }

    private Task OnVoiceServerUpdated(object sender, VoiceServerUpdatedEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(eventArgs);

        if (!_handles.TryGetValue(eventArgs.GuildId, out var playerHandle))
        {
            return Task.CompletedTask;
        }

        _logger.VoiceServerUpdated(eventArgs.GuildId, eventArgs.VoiceServer.Token, eventArgs.VoiceServer.Endpoint);

        return playerHandle.UpdateVoiceServerAsync(eventArgs.VoiceServer).AsTask();
    }

    private Task OnVoiceStateUpdated(object sender, VoiceStateUpdatedEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(eventArgs);

        if (!_handles.TryGetValue(eventArgs.GuildId, out var playerHandle))
        {
            return Task.CompletedTask;
        }

        _logger.VoiceStateUpdated(eventArgs.GuildId, eventArgs.VoiceState.VoiceChannelId, eventArgs.VoiceState.SessionId);

        return playerHandle.UpdateVoiceStateAsync(eventArgs.VoiceState).AsTask();
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (disposing)
        {
            DiscordClient.VoiceStateUpdated -= OnVoiceStateUpdated;
            DiscordClient.VoiceServerUpdated -= OnVoiceServerUpdated;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask<PlayerResult<TPlayer>> RetrieveAsync<TPlayer, TOptions>(ulong guildId, ulong? memberVoiceChannel, PlayerFactory<TPlayer, TOptions> playerFactory, IOptions<TOptions> options, PlayerRetrieveOptions retrieveOptions = default, CancellationToken cancellationToken = default)
        where TPlayer : class, ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        static async ValueTask<PlayerResult<TPlayer>> CheckPreconditionsAsync(
            TPlayer player,
            ImmutableArray<IPlayerPrecondition> preconditions,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(player);

            foreach (var precondition in preconditions)
            {
                if (!await precondition.CheckAsync(player, cancellationToken).ConfigureAwait(false))
                {
                    return PlayerResult<TPlayer>.PreconditionFailed(player, precondition);
                }
            }

            return PlayerResult<TPlayer>.Success(player);
        }

        var player = await GetPlayerAsync<TPlayer>(guildId, cancellationToken).ConfigureAwait(false);

        var channelBehavior = retrieveOptions.ChannelBehavior;
        var voiceStateBehavior = retrieveOptions.VoiceStateBehavior;

        var preconditions = retrieveOptions.Preconditions.IsDefaultOrEmpty
            ? ImmutableArray<IPlayerPrecondition>.Empty
            : retrieveOptions.Preconditions;

        if (player is not null)
        {
            if (voiceStateBehavior is MemberVoiceStateBehavior.AlwaysRequired or MemberVoiceStateBehavior.RequireSame &&
                memberVoiceChannel is null)
            {
                return PlayerResult<TPlayer>.UserNotInVoiceChannel;
            }

            // Player is in a different voice channel than the member
            if (memberVoiceChannel != player.VoiceChannelId)
            {
                if (voiceStateBehavior is MemberVoiceStateBehavior.RequireSame)
                {
                    // It is specified that the user must be in the same channel as the player, fail
                    return PlayerResult<TPlayer>.VoiceChannelMismatch;
                }

                if (memberVoiceChannel is not null && channelBehavior is PlayerChannelBehavior.Move)
                {
                    // It is specified that the player should be moved to the user's channel
                    await DiscordClient
                        .SendVoiceUpdateAsync(guildId, memberVoiceChannel.Value, selfDeaf: options.Value.SelfDeaf, selfMute: options.Value.SelfMute, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
            }

            return await CheckPreconditionsAsync(player, preconditions, cancellationToken).ConfigureAwait(false);
        }

        if (memberVoiceChannel is null)
        {
            return PlayerResult<TPlayer>.UserNotInVoiceChannel;
        }

        var allowConnectToVoiceChannel = channelBehavior is not PlayerChannelBehavior.None;

        if (!allowConnectToVoiceChannel)
        {
            return PlayerResult<TPlayer>.BotNotConnected;
        }

        player = await JoinAsync(guildId, memberVoiceChannel.Value, playerFactory, options, cancellationToken).ConfigureAwait(false);

        return await CheckPreconditionsAsync(player, preconditions, cancellationToken).ConfigureAwait(false);
    }
}

internal static partial class Logging
{
    [LoggerMessage(1, LogLevel.Trace, "Voice server for player '{GuildId}' updated (token: {Token}, endpoint: {Endpoint}).", EventName = nameof(VoiceServerUpdated))]
    public static partial void VoiceServerUpdated(this ILogger<PlayerManager> logger, ulong guildId, string token, string endpoint);

    [LoggerMessage(2, LogLevel.Trace, "Voice state for player '{GuildId}' updated (channel id: {ChannelId}, session id: {SessionId}).", EventName = nameof(VoiceStateUpdated))]
    public static partial void VoiceStateUpdated(this ILogger<PlayerManager> logger, ulong guildId, ulong? channelId, string sessionId);
}