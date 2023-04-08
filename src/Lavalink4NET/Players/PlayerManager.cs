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
using Microsoft.Extensions.Options;

internal sealed class PlayerManager : IPlayerManager, IDisposable
{
    private readonly IDiscordClientWrapper _clientWrapper;
    private readonly ConcurrentDictionary<ulong, ILavalinkPlayerHandle> _handles;
    private readonly ILogger<PlayerManager> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly PlayerContext _playerContext;
    private bool _disposed;

    public PlayerManager(
        IServiceProvider? serviceProvider,
        IDiscordClientWrapper discordClient,
        ILavalinkApiClient apiClient,
        ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(apiClient);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        _handles = new ConcurrentDictionary<ulong, ILavalinkPlayerHandle>();

        _clientWrapper = discordClient;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<PlayerManager>();
        _playerContext = new PlayerContext(serviceProvider, apiClient, discordClient);

        _clientWrapper.VoiceStateUpdated += OnVoiceStateUpdated;
        _clientWrapper.VoiceServerUpdated += OnVoiceServerUpdated;
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

    public ValueTask AssociateAsync(ulong guildId, string sessionId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(sessionId);

        if (!_handles.TryGetValue(guildId, out var handle))
        {
            return default;
        }

        return handle.AssociateAsync(sessionId, cancellationToken);
    }

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

        await _clientWrapper
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

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (disposing)
        {
            _clientWrapper.VoiceStateUpdated -= OnVoiceStateUpdated;
            _clientWrapper.VoiceServerUpdated -= OnVoiceServerUpdated;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
