namespace Lavalink4NET.InactivityTracking;

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Events;
using Lavalink4NET.InactivityTracking.Events;
using Lavalink4NET.InactivityTracking.Players;
using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;
using Lavalink4NET.Tracking;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
///     A service that tracks not-playing players to reduce the usage of the Lavalink nodes.
/// </summary>
public class InactivityTrackingService : IDisposable, IInactivityTrackingService
{
    private readonly IPlayerManager _playerManager;
    private readonly IDiscordClientWrapper _clientWrapper;
    private readonly ISystemClock _systemClock;
    private readonly ILogger<InactivityTrackingService> _logger;
    private readonly InactivityTrackingOptions _options;
    private readonly ConcurrentDictionary<ulong, DateTimeOffset> _players;
    private readonly CancellationTokenSource _stoppingCancellationTokenSource;
    private TaskCompletionSource? _pauseTaskCompletionSource;
    private bool _disposed;
    private Task? _executeTask;

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
        IOptions<InactivityTrackingOptions> options,
        ILogger<InactivityTrackingService> logger)
    {
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(clientWrapper);
        ArgumentNullException.ThrowIfNull(systemClock);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _players = new ConcurrentDictionary<ulong, DateTimeOffset>();
        _stoppingCancellationTokenSource = new CancellationTokenSource();

        _playerManager = playerManager;
        _clientWrapper = clientWrapper;
        _systemClock = systemClock;
        _options = options.Value;
        _logger = logger;

        if (options.Value.TrackInactivity)
        {
            _executeTask = RunAsync(CancellationToken.None).AsTask();
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

    public InactivityTrackingState State => this switch
    {
        { _executeTask: null, } => InactivityTrackingState.Inactive,
        { _executeTask.IsCompleted: true, } => InactivityTrackingState.Stopped,
        { _disposed: true, } => InactivityTrackingState.Destroyed,
        { _pauseTaskCompletionSource: not null, } => InactivityTrackingState.Paused,
        _ => InactivityTrackingState.Running,
    };

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
        if (_systemClock.UtcNow >= dateTimeOffset + _options.DisconnectDelay)
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
    public virtual async ValueTask PollAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        var utcNow = _systemClock.UtcNow;

        var destroyedPlayers = _players.Keys.ToHashSet();

        foreach (var player in _playerManager.Players)
        {
            _ = destroyedPlayers.Remove(player.GuildId);

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

                    if (player is IInactivityPlayerListener inactivityPlayerListener)
                    {
                        await inactivityPlayerListener
                            .NotifyPlayerTrackedAsync(cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
                else if (inactiveSince + _options.DisconnectDelay < utcNow)
                {
                    if (player is IInactivityPlayerListener inactivityPlayerListener)
                    {
                        await inactivityPlayerListener
                            .NotifyPlayerInactiveAsync(cancellationToken)
                            .ConfigureAwait(false);
                    }

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
                    await NotifyAsync(player.GuildId, player, cancellationToken).ConfigureAwait(false);
                }
            }
            else if (_players.TryRemove(player.GuildId, out _))
            {
                if (player is IInactivityPlayerListener inactivityPlayerListener)
                {
                    await inactivityPlayerListener
                        .NotifyPlayerActiveAsync(cancellationToken)
                        .ConfigureAwait(false);
                }

                _logger.LogDebug("Removed player {GuildId} from tracking list.", player.GuildId);

                // remove from tracking list
                await NotifyAsync(player.GuildId, player, cancellationToken).ConfigureAwait(false);
            }
        }

        // Untrack destroyed players
        foreach (var destroyedPlayer in destroyedPlayers)
        {
            if (_players.TryRemove(destroyedPlayer, out _))
            {
                _logger.LogDebug("Removed player {GuildId} from tracking list.", destroyedPlayer);

                // remove from tracking list
                await NotifyAsync(destroyedPlayer, player: null, cancellationToken).ConfigureAwait(false);
            }
        }
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
    public async ValueTask NotifyAsync(ulong guildId, ILavalinkPlayer? player, CancellationToken cancellationToken = default)
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

        var context = new InactivityTrackingContext(
            InactivityTrackingService: this,
            Client: _clientWrapper,
            Player: player);

        if (_options.Mode is InactivityTrackingMode.Any)
        {
            // Any of the trackers need to mark the player as inactive to be considered inactive
            foreach (var tracker in _options.Trackers)
            {
                if (await tracker.CheckAsync(context, cancellationToken).ConfigureAwait(false))
                {
                    return true;
                }
            }

            return false;
        }
        else
        {
            // All trackers need to mark the player as inactive to be considered inactive
            foreach (var tracker in _options.Trackers)
            {
                if (!await tracker.CheckAsync(context, cancellationToken).ConfigureAwait(false))
                {
                    return false;
                }
            }

            return true;
        }
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

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _stoppingCancellationTokenSource.Dispose();
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
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(LavalinkPlayer));
        }
#endif
    }

    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        if (_executeTask is not null)
        {
            if (_executeTask.IsCompleted)
            {
                throw new InvalidOperationException("The inactivity tracking service was stopped.");
            }

            return ValueTask.CompletedTask;
        }

        _executeTask = RunAsync(cancellationToken).AsTask();

        return _executeTask.IsCompleted ? new ValueTask(_executeTask) : ValueTask.CompletedTask;
    }

    public async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_executeTask is null)
        {
            return;
        }

        try
        {
            _pauseTaskCompletionSource?.SetCanceled(CancellationToken.None);
            _stoppingCancellationTokenSource.Cancel();
        }
        finally
        {
            await _executeTask
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    public ValueTask PauseAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Interlocked.CompareExchange(
            location1: ref _pauseTaskCompletionSource,
            value: new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously),
            comparand: null);

        return ValueTask.CompletedTask;
    }

    public ValueTask ResumeAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var pauseTaskCompletionSource = Interlocked.Exchange(ref _pauseTaskCompletionSource, null);
        pauseTaskCompletionSource?.TrySetResult();

        return ValueTask.CompletedTask;
    }

    public async ValueTask RunAsync(CancellationToken cancellationToken = default)
    {
        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            token1: cancellationToken,
            token2: _stoppingCancellationTokenSource.Token);

        cancellationToken = cancellationTokenSource.Token;

        using var periodicTimer = new PeriodicTimer(_options.PollInterval);

        try
        {
            while (await periodicTimer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                await PollAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
    }
}
