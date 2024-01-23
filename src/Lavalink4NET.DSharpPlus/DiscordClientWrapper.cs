namespace Lavalink4NET.DSharpPlus;

using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using global::DSharpPlus;
using global::DSharpPlus.Entities;
using global::DSharpPlus.EventArgs;
using global::DSharpPlus.Exceptions;
using global::DSharpPlus.Net.Abstractions;
using Lavalink4NET.Clients;
using Lavalink4NET.Clients.Events;
using Lavalink4NET.Events;
using Microsoft.Extensions.Logging;

/// <summary>
/// Wraps a <see cref="DiscordClient"/> or <see cref="DiscordShardedClient"/> instance.
/// </summary>
public sealed class DiscordClientWrapper : IDiscordClientWrapper, IDisposable
{
    /// <inheritdoc/>
    public event AsyncEventHandler<VoiceServerUpdatedEventArgs>? VoiceServerUpdated;

    /// <inheritdoc/>
    public event AsyncEventHandler<VoiceStateUpdatedEventArgs>? VoiceStateUpdated;

    private readonly object _client; // either DiscordShardedClient or DiscordClient
    private readonly ILogger<DiscordClientWrapper> _logger;
    private readonly TaskCompletionSource<ClientInformation> _readyTaskCompletionSource;
    private bool _disposed;

    private DiscordClientWrapper(object discordClient, ILogger<DiscordClientWrapper> logger)
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(logger);

        _client = discordClient;
        _logger = logger;

        _readyTaskCompletionSource = new TaskCompletionSource<ClientInformation>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>
    /// Creates a new instance of <see cref="DiscordClientWrapper"/>.
    /// </summary>
    /// <param name="discordClient">The Discord Client to wrap.</param>
    /// <param name="logger">a logger associated with this wrapper.</param>
    public DiscordClientWrapper(DiscordClient discordClient, ILogger<DiscordClientWrapper> logger)
        : this((object)discordClient, logger)
    {
        ArgumentNullException.ThrowIfNull(discordClient);

        discordClient.VoiceStateUpdated += OnVoiceStateUpdated;
        discordClient.VoiceServerUpdated += OnVoiceServerUpdated;
        discordClient.Ready += OnClientReady;
    }

    /// <summary>
    /// Creates a new instance of <see cref="DiscordClientWrapper"/>.
    /// </summary>
    /// <param name="shardedDiscordClient">The Sharded Discord Client to wrap.</param>
    /// <param name="logger">a logger associated with this wrapper.</param>
    public DiscordClientWrapper(DiscordShardedClient shardedDiscordClient, ILogger<DiscordClientWrapper> logger)
        : this((object)shardedDiscordClient, logger)
    {
        ArgumentNullException.ThrowIfNull(shardedDiscordClient);

        shardedDiscordClient.VoiceStateUpdated += OnVoiceStateUpdated;
        shardedDiscordClient.VoiceServerUpdated += OnVoiceServerUpdated;
        shardedDiscordClient.Ready += OnClientReady;
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public async ValueTask<ImmutableArray<ulong>> GetChannelUsersAsync(
        ulong guildId,
        ulong voiceChannelId,
        bool includeBots = false,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        DiscordChannel channel;
        try
        {
            channel = await GetClientForGuild(guildId)
                .GetChannelAsync(voiceChannelId)
                .ConfigureAwait(false);

            if (channel is null)
            {
                return ImmutableArray<ulong>.Empty;
            }
        }
        catch (DiscordException exception)
        {
            _logger.LogWarning(
                exception, "An error occurred while retrieving the users for voice channel '{VoiceChannelId}' of the guild '{GuildId}'.",
                voiceChannelId, guildId);

            return ImmutableArray<ulong>.Empty;
        }

        var filteredUsers = ImmutableArray.CreateBuilder<ulong>(channel.Users.Count);

        foreach (var member in channel.Users)
        {
            // Always skip the current user.
            // If we're not including bots and the member is a bot, skip them.
            if (!member.IsCurrent || includeBots || !member.IsBot)
            {
                filteredUsers.Add(member.Id);
            }
        }

        return filteredUsers.ToImmutable();
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public async ValueTask SendVoiceUpdateAsync(
        ulong guildId,
        ulong? voiceChannelId,
        bool selfDeaf = false,
        bool selfMute = false,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var client = GetClientForGuild(guildId);

        var payload = new VoiceStateUpdatePayload(
            guildId: guildId,
            channelId: voiceChannelId,
            isSelfMuted: selfMute,
            isSelfDeafened: selfDeaf);

#pragma warning disable CS0618 // This method should not be used unless you know what you're doing. Instead, look towards the other explicitly implemented methods which come with client-side validation.
        // Jan 23, 2024, OoLunar: We're telling Discord that we're joining a voice channel.
        // At the time of writing, both DSharpPlus.VoiceNext and DSharpPlus.VoiceLink™
        // use this method to send voice state updates.
        await client
            .SendPayloadAsync(GatewayOpCode.VoiceStateUpdate, JsonSerializer.Serialize(payload))
            .ConfigureAwait(false);
#pragma warning restore CS0618 // This method should not be used unless you know what you're doing. Instead, look towards the other explicitly implemented methods which come with client-side validation.
    }

    /// <inheritdoc/>
    public ValueTask<ClientInformation> WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return new(_readyTaskCompletionSource.Task.WaitAsync(cancellationToken));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_client is DiscordClient discordClient)
        {
            discordClient.VoiceStateUpdated -= OnVoiceStateUpdated;
            discordClient.VoiceServerUpdated -= OnVoiceServerUpdated;
            discordClient.Ready -= OnClientReady;
        }
        else if (_client is DiscordShardedClient shardedClient)
        {
            shardedClient.VoiceStateUpdated -= OnVoiceStateUpdated;
            shardedClient.VoiceServerUpdated -= OnVoiceServerUpdated;
            shardedClient.Ready -= OnClientReady;
        }
    }

    private DiscordClient GetClientForGuild(ulong guildId) => _client is DiscordClient discordClient
        ? discordClient
        : ((DiscordShardedClient)_client).GetShard(guildId);

    private Task OnClientReady(DiscordClient discordClient, ReadyEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(eventArgs);

        var clientInformation = new ClientInformation(
            Label: "DSharpPlus",
            CurrentUserId: discordClient.CurrentUser.Id,
            ShardCount: discordClient.ShardCount);

        _readyTaskCompletionSource.TrySetResult(clientInformation);
        return Task.CompletedTask;
    }

    private async Task OnVoiceServerUpdated(DiscordClient discordClient, VoiceServerUpdateEventArgs voiceServerUpdateEventArgs)
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(voiceServerUpdateEventArgs);

        var server = new VoiceServer(
            Token: voiceServerUpdateEventArgs.VoiceToken,
            Endpoint: voiceServerUpdateEventArgs.Endpoint);

        var eventArgs = new VoiceServerUpdatedEventArgs(
            guildId: voiceServerUpdateEventArgs.Guild.Id,
            voiceServer: server);

        await VoiceServerUpdated
            .InvokeAsync(this, eventArgs)
            .ConfigureAwait(false);
    }

    private async Task OnVoiceStateUpdated(DiscordClient discordClient, VoiceStateUpdateEventArgs voiceStateUpdateEventArgs)
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(voiceStateUpdateEventArgs);

        // session id is the same as the resume key so DSharpPlus should be able to give us the
        // session key in either before or after voice state
        var sessionId = voiceStateUpdateEventArgs.Before?.GetSessionId() ?? voiceStateUpdateEventArgs.After.GetSessionId();

        // create voice state
        var voiceState = new VoiceState(
            VoiceChannelId: voiceStateUpdateEventArgs.After?.Channel?.Id,
            SessionId: sessionId);

        var oldVoiceState = new VoiceState(
            VoiceChannelId: voiceStateUpdateEventArgs.Before?.Channel?.Id,
            SessionId: sessionId);

        // invoke event
        var eventArgs = new VoiceStateUpdatedEventArgs(
            guildId: voiceStateUpdateEventArgs.Guild.Id,
            userId: voiceStateUpdateEventArgs.User.Id,
            isCurrentUser: voiceStateUpdateEventArgs.User.Id == discordClient.CurrentUser.Id,
            oldVoiceState: oldVoiceState,
            voiceState: voiceState);

        await VoiceStateUpdated
            .InvokeAsync(this, eventArgs)
            .ConfigureAwait(false);
    }
}
