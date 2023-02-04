namespace Lavalink4NET.Players;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Clients.Events;
using Lavalink4NET.Rest;
using Microsoft.Extensions.Logging;

internal sealed class PlayerManager : IPlayerManager
{
    private readonly ILavalinkApiClient _apiClient;
    private readonly TaskCompletionSource<string> _sessionIdTaskCompletionSource;
    private readonly ILogger<PlayerManager> _logger;
    private readonly IDiscordClientWrapper _clientWrapper;
    private readonly ConcurrentDictionary<ulong, LavalinkPlayerHandle> _handles;

    public PlayerManager(
        IDiscordClientWrapper clientWrapper,
        ILavalinkApiClient apiClient,
        TaskCompletionSource<string> sessionIdTaskCompletionSource,
        ILogger<PlayerManager> logger)
    {
        ArgumentNullException.ThrowIfNull(clientWrapper);
        ArgumentNullException.ThrowIfNull(apiClient);
        ArgumentNullException.ThrowIfNull(sessionIdTaskCompletionSource);
        ArgumentNullException.ThrowIfNull(logger);

        _handles = new ConcurrentDictionary<ulong, LavalinkPlayerHandle>();

        _clientWrapper = clientWrapper;
        _apiClient = apiClient;
        _sessionIdTaskCompletionSource = sessionIdTaskCompletionSource;
        _logger = logger;

        _clientWrapper.VoiceStateUpdated += OnVoiceStateUpdated;
        _clientWrapper.VoiceServerUpdated += OnVoiceServerUpdated; // TODO: unsubscribe on dispose
    }

    private Task OnVoiceServerUpdated(object sender, VoiceServerUpdatedEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(eventArgs);

        if (!_handles.TryGetValue(eventArgs.GuildId, out var playerHandle))
        {
            return Task.CompletedTask;
        }

        _logger.LogTrace(
            "Voice server for player '{GuildId}' updated (token: {Token}, endpoint: {Endpoint}).",
            eventArgs.GuildId, eventArgs.VoiceServer.Token, eventArgs.VoiceServer.Endpoint);

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

        _logger.LogTrace(
            "Voice state for player '{GuildId}' updated (channel id: {ChannelId}, session id: {SessionId}).",
            eventArgs.GuildId, eventArgs.VoiceState.VoiceChannelId, eventArgs.VoiceState.SessionId);

        return playerHandle.UpdateVoiceServerAsync(eventArgs.VoiceState).AsTask();
    }

    public IEnumerable<ILavalinkPlayer> Players
    {
        get
        {
            // TODO: check destroyed
            return _handles.Values
                .Select(x => x.Player)
                .Where(x => x is not null)!;
        }
    }

    public async ValueTask<T?> GetPlayerAsync<T>(ulong guildId, CancellationToken cancellationToken = default) where T : class, ILavalinkPlayer
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await GetPlayerAsync(guildId, cancellationToken) as T;
    }

    public async ValueTask<ILavalinkPlayer?> GetPlayerAsync(ulong guildId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_handles.TryGetValue(guildId, out var handle))
        {
            return null;
        }

        // TODO: check if destroyed

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
        // TODO: check destroyed
        return _handles.ContainsKey(guildId);
    }
    public async ValueTask<T> JoinAsync<T>(ulong guildId, ulong voiceChannelId, PlayerFactory<T> playerFactory, PlayerJoinOptions options = default, CancellationToken cancellationToken = default) where T : ILavalinkPlayer
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sessionId = await _sessionIdTaskCompletionSource.Task
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        LavalinkPlayerHandle Create(ulong guildId)
        {
            return new LavalinkPlayerHandle(
                guildId: guildId,
                client: _clientWrapper,
                apiClient: _apiClient,
                sessionId: sessionId,
                playerFactory: x => playerFactory(x));
        }

        var handle = _handles.GetOrAdd(guildId, Create);

        await _clientWrapper
            .SendVoiceUpdateAsync(guildId, voiceChannelId, selfDeaf: options.SelfDeaf, selfMute: options.SelfMute, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        // TODO: throw on mismatch
        return (T)await handle
            .GetPlayerAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public ValueTask<ILavalinkPlayer> JoinAsync(ulong guildId, ulong voiceChannelId, PlayerFactory<ILavalinkPlayer> playerFactory, PlayerJoinOptions options = default, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerFactory);

        return JoinAsync<ILavalinkPlayer>(
            guildId: guildId,
            voiceChannelId: voiceChannelId,
            playerFactory: playerFactory,
            options: options,
            cancellationToken: cancellationToken);
    }

    public ValueTask<ILavalinkPlayer> JoinAsync(ulong guildId, ulong voiceChannelId, PlayerJoinOptions options = default, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return JoinAsync<ILavalinkPlayer>(
            guildId: guildId,
            voiceChannelId: voiceChannelId,
            playerFactory: static properties => new LavalinkPlayer(properties),
            options: options,
            cancellationToken: cancellationToken);
    }
}
