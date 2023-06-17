namespace Lavalink4NET.Tracking;

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Events;
using Lavalink4NET.InactivityTracking.Events;
using Lavalink4NET.Players;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

/// <summary>
///     A service that tracks not-playing players to reduce the usage of the Lavalink nodes.
/// </summary>
public class InactivityTrackingService : IDisposable
{
    private readonly IPlayerManager _playerManager;
    private readonly IDiscordClientWrapper _clientWrapper;
    private readonly ISystemClock _systemClock;
    private readonly ILogger<InactivityTrackingService> _logger;
    private readonly InactivityTrackingOptions _options;
    private readonly ConcurrentDictionary<ulong, DateTimeOffset> _players;
    private readonly ImmutableArray<InactivityTracker> _trackers;
    private Timer? _timer;
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="InactivityTrackingService"/> class.
    /// </summary>
    /// <param name="audioService">the audio service where the players should be tracked</param>
    /// <param name="clientWrapper">the discord client wrapper</param>
    /// <param name="options">the tracking options</param>
    /// <param name="logger">the optional logger</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="audioService"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="clientWrapper"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public InactivityTrackingService(
        IPlayerManager playerManager,
        IDiscordClientWrapper clientWrapper,
        ISystemClock systemClock,
        InactivityTrackingOptions options,
        ILogger<InactivityTrackingService> logger)
    {
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(clientWrapper);
        ArgumentNullException.ThrowIfNull(systemClock);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _playerManager = playerManager;
        _clientWrapper = clientWrapper;
        _systemClock = systemClock;
        _options = options;
        _logger = logger;
        _players = new ConcurrentDictionary<ulong, DateTimeOffset>();
        _trackers = options.Trackers;

        if (options.TrackInactivity)
        {
            Start();
        }
    }

    /// <summary>
    ///     An asynchronously event that is triggered when an inactive player was found.
    /// </summary>
    public event AsyncEventHandler<InactivePlayerEventArgs>? InactivePlayer;

    /// <summary>
    ///     An asynchronously event that is triggered when a player's tracking status ( <see
    ///     cref="InactivityTrackingStatus"/>) was updated.
    /// </summary>
    public event AsyncEventHandler<PlayerTrackingStatusUpdateEventArgs>? PlayerTrackingStatusUpdated;

    /// <summary>
    ///     Gets a value indicating whether the service is tracking inactive players.
    /// </summary>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public bool IsTracking
    {
        get
        {
            ThrowIfDisposed();
            return _timer is not null;
        }
    }

    /// <summary>
    ///     Beings tracking of inactive players.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the service is already tracking inactive players.
    /// </exception>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public void Start()
    {
        ThrowIfDisposed();

        if (_timer is not null)
        {
            return;
        }

        // initialize the timer that polls inactive players
        var pollDelay = _options.DelayFirstTrack ? _options.PollInterval : TimeSpan.Zero;
        _timer = new Timer(PollTimerCallback, this, pollDelay, _options.PollInterval);
    }

    /// <summary>
    ///     Gets the tracking status of the specified <paramref name="player"/>.
    /// </summary>
    /// <param name="player">the player</param>
    /// <returns>the inactivity tracking status of the player</returns>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public InactivityTrackingStatus GetStatus(ILavalinkPlayer player)
    {
        ThrowIfDisposed();

        if (!_players.TryGetValue(player.GuildId, out var dateTimeOffset))
        {
            // there are no tracking entries for the player
            return InactivityTrackingStatus.NotTracked;
        }

        // the player has exceeded the stop delay
        if (_systemClock.UtcNow > dateTimeOffset)
        {
            return InactivityTrackingStatus.Inactive;
        }

        // player is tracked for inactivity, but not removed
        return InactivityTrackingStatus.Tracked;
    }

    /// <summary>
    ///     Force polls tracking of all inactive players asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public virtual async Task PollAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        var utcNow = _systemClock.UtcNow;
        var destroyedPlayers = _players.Keys.ToHashSet();

        foreach (var player in _playerManager.Players)
        {
            var result = destroyedPlayers.Remove(player.GuildId);
            Debug.Assert(result);

            // check if the player is inactive
            if (await IsInactiveAsync(player, cancellationToken).ConfigureAwait(false))
            {
                var inactiveSince = _players.GetOrAdd(player.GuildId, utcNow);

                // add the player to tracking list
                if (inactiveSince == utcNow)
                {
                    _logger.LogDebug("Tracked player {GuildId} as inactive.", player.GuildId);

                    // trigger event
                    var eventArgs = new PlayerTrackingStatusUpdateEventArgs(
                        playerManager: _playerManager,
                        guildId: player.GuildId,
                        player: player,
                        trackingStatus: InactivityTrackingStatus.Tracked);

                    await OnPlayerTrackingStatusUpdated(eventArgs).ConfigureAwait(false);
                }
                else if (inactiveSince + _options.DisconnectDelay < utcNow)
                {
                    // trigger event
                    var eventArgs = new InactivePlayerEventArgs(_playerManager, player);
                    await OnInactivePlayerAsync(eventArgs).ConfigureAwait(false);

                    // it is wanted that the player should not stop.
                    if (!eventArgs.ShouldStop)
                    {
                        continue;
                    }

                    _logger.LogDebug("Destroyed player {GuildId} due inactivity.", player.GuildId);

                    await using var _ = player.ConfigureAwait(false);
                    await UntrackPlayerAsync(player.GuildId, player, cancellationToken).ConfigureAwait(false);
                }
            }
            else if (_players.TryRemove(player.GuildId, out _))
            {
                _logger.LogDebug("Removed player {GuildId} from tracking list.", player.GuildId);

                // remove from tracking list
                await UntrackPlayerAsync(player.GuildId, player, cancellationToken).ConfigureAwait(false);
            }
        }

        // Untrack destroyed players
        foreach (var destroyedPlayer in destroyedPlayers)
        {
            if (_players.TryRemove(destroyedPlayer, out _))
            {
                _logger.LogDebug("Removed player {GuildId} from tracking list.", destroyedPlayer);

                // remove from tracking list
                await UntrackPlayerAsync(destroyedPlayer, player: null, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    ///     Stops tracking of inactive players.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the service is not tracking inactive players.
    /// </exception>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public void Stop()
    {
        ThrowIfDisposed();

        _timer?.Dispose();
        _timer = null;
    }

    /// <summary>
    ///     Removes the specified <paramref name="player"/> from the inactivity tracking list asynchronously.
    /// </summary>
    /// <param name="player">the player to remove</param>
    /// <returns>
    ///     a task that represents the asynchronous operation. The task result is a value
    ///     indicating whether the player was removed from the tracking list.
    /// </returns>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public async ValueTask UntrackPlayerAsync(ulong guildId, ILavalinkPlayer? player, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (_players.TryRemove(guildId, out _))
        {
            // trigger event
            var eventArgs = new PlayerTrackingStatusUpdateEventArgs(
                playerManager: _playerManager,
                guildId: guildId,
                player: player,
                trackingStatus: InactivityTrackingStatus.NotTracked);

            await OnPlayerTrackingStatusUpdated(eventArgs).ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Gets a value indicating whether the specified <paramref name="player"/> is inactive asynchronously.
    /// </summary>
    /// <param name="player">the player to check</param>
    /// <returns>
    ///     a task that represents the asynchronous operation. The task result is a value
    ///     indicating whether the specified <paramref name="player"/> is inactive.
    /// </returns>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    protected virtual async ValueTask<bool> IsInactiveAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        // iterate through the trackers
        foreach (var tracker in _trackers)
        {
            // check if the player is inactivity
            if (await tracker(player, _clientWrapper, cancellationToken).ConfigureAwait(false))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Triggers the <see cref="InactivePlayer"/> event asynchronously.
    /// </summary>
    /// <param name="eventArgs">the event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    protected virtual ValueTask OnInactivePlayerAsync(InactivePlayerEventArgs eventArgs)
        => InactivePlayer.InvokeAsync(this, eventArgs);

    /// <summary>
    ///     Triggers the <see cref="PlayerTrackingStatusUpdated"/> event asynchronously.
    /// </summary>
    /// <param name="eventArgs">the event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    protected virtual ValueTask OnPlayerTrackingStatusUpdated(PlayerTrackingStatusUpdateEventArgs eventArgs)
        => PlayerTrackingStatusUpdated.InvokeAsync(this, eventArgs);

    private void PollTimerCallback(object? state)
    {
        Debug.Assert(state is InactivityTrackingService);
        var instance = Unsafe.As<object?, InactivityTrackingService>(ref state);

        try
        {
            instance.PollAsync().GetAwaiter().GetResult();
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Inactivity tracking poll failed!");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _timer?.Dispose();
            _timer = null;
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected void ThrowIfDisposed()
    {
#if NET7_0_OR_GREATER
        ThrowIfDisposed();
#else
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(LavalinkPlayer));
        }
#endif
    }
}
