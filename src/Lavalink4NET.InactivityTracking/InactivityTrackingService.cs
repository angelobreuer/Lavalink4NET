namespace Lavalink4NET.InactivityTracking;

using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
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
    private readonly InactivityTrackingContext _trackingContext;
    private readonly IPlayerManager _playerManager;
    private readonly ISystemClock _systemClock;
    private readonly ILogger<InactivityTrackingService> _logger;
    private readonly InactivityTrackingOptions _options;
    private readonly ImmutableArray<IInactivityTracker> _trackers;
    private readonly Dictionary<ulong, PlayerTrackingMap> _players;
    private readonly SemaphoreSlim _playersSemaphoreSlim;
    private readonly CancellationTokenSource _stoppingCancellationTokenSource;
    private readonly PlayerTrackingState _defaultTrackingState;
    private TaskCompletionSource? _pauseTaskCompletionSource;
    private bool _disposed;
    private Task? _executeTask;

    /// <summary>
    ///     Initializes a new instance of the <see cref="InactivityTrackingService"/> class.
    /// </summary>
    /// <param name="audioService">the audio service where the players should be tracked</param>
    /// <param name="discordClient">the discord client wrapper</param>
    /// <param name="options">the tracking options</param>
    /// <param name="logger">the optional logger</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="audioService"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="discordClient"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public InactivityTrackingService(
        IPlayerManager playerManager,
        IDiscordClientWrapper discordClient,
        ISystemClock systemClock,
        IEnumerable<IInactivityTracker> trackers,
        IOptions<InactivityTrackingOptions> options,
        ILogger<InactivityTrackingService> logger)
    {
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(systemClock);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _players = new Dictionary<ulong, PlayerTrackingMap>();
        _playersSemaphoreSlim = new SemaphoreSlim(1, 1);
        _stoppingCancellationTokenSource = new CancellationTokenSource();

        _trackers = options.Value.Trackers is null
            ? trackers.ToImmutableArray()
            : options.Value.Trackers.Value;

        _playerManager = playerManager;
        _systemClock = systemClock;
        _options = options.Value;
        _logger = logger;

        _trackingContext = new InactivityTrackingContext(this, discordClient);
        _defaultTrackingState = CreateDefaultTrackingState(_trackers);

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
    ///     cref="PlayerTrackingStatus"/>) was updated.
    /// </summary>
    public event AsyncEventHandler<TrackingStatusChangedEventArgs>? TrackingStatusChanged;

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
    public async ValueTask<PlayerTrackingState> GetPlayerAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        await _playersSemaphoreSlim
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            return _players.TryGetValue(player.GuildId, out var trackingMap)
                ? CreateTrackingState(trackingMap, _systemClock.UtcNow)
                : _defaultTrackingState;
        }
        finally
        {
            _playersSemaphoreSlim.Release();
        }
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

        var destroyedPlayers = _players.Keys.ToHashSet();

        await _playersSemaphoreSlim
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            foreach (var player in _playerManager.Players)
            {
                var activityStatus = await CheckAsync(player, cancellationToken).ConfigureAwait(false);

                if (activityStatus is not PlayerActivityStatus.Inactive)
                {
                    _ = destroyedPlayers.Remove(player.GuildId);
                    continue;
                }

                var eventArgs = new InactivePlayerEventArgs(_playerManager, player);
                await OnInactivePlayerAsync(eventArgs).ConfigureAwait(false);

                if (eventArgs.ShouldStop)
                {
                    await using var _ = player.ConfigureAwait(false);
                    await player.DisconnectAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            _playersSemaphoreSlim.Release();
        }

        // Untrack destroyed players
        foreach (var destroyedPlayer in destroyedPlayers)
        {
            if (_players.Remove(destroyedPlayer, out _))
            {
                _logger.RemovedPlayerFromTrackingList(destroyedPlayer);
            }
        }
    }

    protected virtual async ValueTask<PlayerActivityStatus> CheckAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(player);

        var utcNow = _systemClock.UtcNow;

        var computedEntryMap = new PlayerTrackingMap(
            entries: new PlayerTrackingMapEntry[_trackers.Length],
            computedAt: utcNow);

        for (var trackerIndex = 0; trackerIndex < _trackers.Length; trackerIndex++)
        {
            var inactivityTracker = _trackers[trackerIndex];

            var result = await inactivityTracker
                .CheckAsync(_trackingContext, player, cancellationToken)
                .ConfigureAwait(false);

            computedEntryMap.GetEntry(trackerIndex) = result.Status is PlayerActivityStatus.Active
                ? default
                : new PlayerTrackingMapEntry(utcNow, result.Timeout ?? _options.DefaultTimeout);
        }

        PlayerTrackingMap? Apply(out PlayerTrackingStatus previousStatus)
        {
            ref var presentEntryMap = ref CollectionsMarshal.GetValueRefOrAddDefault(_players, player.GuildId, out var exists);

            if (exists)
            {
                previousStatus = presentEntryMap.Compute(presentEntryMap.ComputedAt, out _);

                var previousMap = presentEntryMap;
                presentEntryMap = computedEntryMap.Apply(presentEntryMap, utcNow);
                return previousMap;
            }
            else
            {
                previousStatus = PlayerTrackingStatus.NotTracked;
                presentEntryMap = computedEntryMap;
                return null;
            }
        }

        var presentEntryMap = Apply(out var previousStatus);
        var currentStatus = computedEntryMap.Compute(utcNow, out var inactiveTrackerIndex);

        if (previousStatus == currentStatus)
        {
            return currentStatus is PlayerTrackingStatus.Inactive
                ? PlayerActivityStatus.Inactive
                : PlayerActivityStatus.Active;
        }

        var previousTrackingState = presentEntryMap is null
            ? _defaultTrackingState
            : CreateTrackingState(presentEntryMap.Value, utcNow);

        var currentTrackingState = CreateTrackingState(
            entryMap: computedEntryMap,
            utcNow: utcNow);

        if (player is IInactivityPlayerListener playerListener)
        {
            var task = currentStatus switch
            {
                PlayerTrackingStatus.NotTracked => playerListener.NotifyPlayerActiveAsync(
                    trackingState: currentTrackingState,
                    previousTrackingState: previousTrackingState,
                    cancellationToken: cancellationToken),

                PlayerTrackingStatus.Tracked => playerListener.NotifyPlayerTrackedAsync(
                    trackingState: currentTrackingState,
                    previousTrackingState: previousTrackingState,
                    cancellationToken: cancellationToken),

                PlayerTrackingStatus.Inactive => playerListener.NotifyPlayerInactiveAsync(
                    trackingState: currentTrackingState,
                    previousTrackingState: previousTrackingState,
                    inactivityTracker: _trackers[inactiveTrackerIndex],
                    cancellationToken: cancellationToken),

                _ => throw new NotSupportedException(),
            };

            await task.ConfigureAwait(false);
        }

        var eventArgs = new TrackingStatusChangedEventArgs(
            playerManager: _playerManager,
            player: player,
            trackingState: currentTrackingState,
            previousTrackingState: previousTrackingState);

        await OnTrackingStatusChangedAsync(eventArgs).ConfigureAwait(false);

        return currentStatus is PlayerTrackingStatus.Inactive
            ? PlayerActivityStatus.Inactive
            : PlayerActivityStatus.Active;
    }

    /// <summary>
    ///     Triggers the <see cref="InactivePlayer"/> event asynchronously.
    /// </summary>
    /// <param name="eventArgs">the event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    protected virtual ValueTask OnInactivePlayerAsync(InactivePlayerEventArgs eventArgs)
        => InactivePlayer.InvokeAsync(this, eventArgs);

    /// <summary>
    ///     Triggers the <see cref="TrackingStatusChanged"/> event asynchronously.
    /// </summary>
    /// <param name="eventArgs">the event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    protected virtual ValueTask OnTrackingStatusChangedAsync(TrackingStatusChangedEventArgs eventArgs)
        => TrackingStatusChanged.InvokeAsync(this, eventArgs);

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

    private PlayerTrackingState CreateTrackingState(PlayerTrackingMap entryMap, DateTimeOffset utcNow)
    {
        PlayerTrackerInformation CreateTrackerInformation(IInactivityTracker tracker, int index)
        {
            var entry = entryMap.GetEntry(index);

            return new PlayerTrackerInformation(
                Tracker: tracker,
                Status: entry.Compute(utcNow),
                TrackedSince: entry.TrackedSince,
                Timeout: entry.Timeout);
        }

        return new PlayerTrackingState(
            Status: entryMap.Compute(utcNow, out _),
            Trackers: _trackers.Select(CreateTrackerInformation).ToImmutableArray());
    }

    private static PlayerTrackingState CreateDefaultTrackingState(ImmutableArray<IInactivityTracker> trackers)
    {
        var trackerInformation = trackers
            .Select(tracker => new PlayerTrackerInformation(tracker, PlayerTrackingStatus.NotTracked))
            .ToImmutableArray();

        return new PlayerTrackingState(
            Status: PlayerTrackingStatus.NotTracked,
            Trackers: trackerInformation);
    }
}

internal record struct PlayerTrackingMap
{
    private readonly PlayerTrackingMapEntry[] _entries;

    public PlayerTrackingMap(PlayerTrackingMapEntry[] entries, DateTimeOffset computedAt)
    {
        _entries = entries;
        ComputedAt = computedAt;
    }

    public readonly ref PlayerTrackingMapEntry GetEntry(int trackerIndex)
    {
        return ref _entries[trackerIndex];
    }

    public DateTimeOffset ComputedAt { get; set; }

    public readonly PlayerTrackingStatus Compute(DateTimeOffset utcNow, out int inactiveTrackerIndex)
    {
        inactiveTrackerIndex = 0;

        var tracked = false;
        var inactive = false;

        var inactiveTimeout = default(TimeSpan?);

        for (var trackerIndex = 0; trackerIndex < _entries.Length; trackerIndex++)
        {
            var entry = _entries[trackerIndex];
            var trackedSince = entry.TrackedSince;

            if (trackedSince is not null)
            {
                tracked = true;

                if (utcNow >= trackedSince.Value + entry.Timeout)
                {
                    if (inactiveTimeout is null || entry.Timeout > inactiveTimeout)
                    {
                        inactiveTimeout = entry.Timeout;
                        inactiveTrackerIndex = trackerIndex;
                    }

                    inactive = true;
                }
            }
        }

        if (inactive)
        {
            return PlayerTrackingStatus.Inactive;
        }

        if (tracked)
        {
            return PlayerTrackingStatus.Tracked;
        }

        return PlayerTrackingStatus.NotTracked;
    }

    public readonly PlayerTrackingMap Apply(PlayerTrackingMap presentMap, DateTimeOffset utcNow)
    {
        for (var trackerIndex = 0; trackerIndex < _entries.Length; trackerIndex++)
        {
            ref var computedEntry = ref _entries[trackerIndex];
            var presentEntry = presentMap.GetEntry(trackerIndex);

            /*if (presentEntry.IsTracked)
            {
                // Entry is already tracked, check if the timeout has elapsed
                computedEntry = presentEntry.TrackedSince + presentEntry.Timeout < utcNow
                    ? new PlayerTrackingMapEntry(presentEntry.TrackedSince, computedEntry.Timeout)
                    : presentEntry; // Timeout has not elapsed, use the present entry
            }
            else
            {
                // Entry is not tracked, check if the player is inactive
                computedEntry = computedEntry.IsTracked
                    ? new PlayerTrackingMapEntry(computedEntry.TrackedSince, computedEntry.Timeout) // Player is inactive, update the entry
                    : presentEntry; // Player is active, use the present entry
            }*/

            computedEntry = computedEntry.IsTracked
                ? new PlayerTrackingMapEntry(presentEntry.TrackedSince ?? computedEntry.TrackedSince, computedEntry.Timeout)
                : computedEntry;
        }

        return this;
    }
}

internal readonly record struct PlayerTrackingMapEntry
{
    private readonly DateTimeOffset _trackedSince;

    public PlayerTrackingMapEntry(DateTimeOffset? trackedSince, TimeSpan timeout)
    {
        _trackedSince = trackedSince is null
            ? default
            : trackedSince.Value;

        Timeout = timeout;
    }

    public PlayerTrackingStatus Compute(DateTimeOffset utcNow)
    {
        if (_trackedSince == default)
        {
            return PlayerTrackingStatus.NotTracked;
        }

        if (utcNow >= _trackedSince + Timeout)
        {
            return PlayerTrackingStatus.Inactive;
        }

        return PlayerTrackingStatus.Tracked;
    }

    public bool IsTracked => _trackedSince != default;

    public DateTimeOffset? TrackedSince => _trackedSince == default
        ? null
        : _trackedSince;

    public TimeSpan Timeout { get; }
}

internal static partial class Logger
{
    [LoggerMessage(1, LogLevel.Debug, "Tracked player {GuildId} as inactive.", EventName = nameof(TrackedPlayerAsInactive))]
    public static partial void TrackedPlayerAsInactive(this ILogger<InactivityTrackingService> logger, ulong guildId);

    [LoggerMessage(2, LogLevel.Debug, "Destroyed player {GuildId} due inactivity.", EventName = nameof(DestroyedPlayerDueInactivity))]
    public static partial void DestroyedPlayerDueInactivity(this ILogger<InactivityTrackingService> logger, ulong guildId);

    [LoggerMessage(3, LogLevel.Debug, "Removed player {GuildId} from tracking list.", EventName = nameof(RemovedPlayerFromTrackingList))]
    public static partial void RemovedPlayerFromTrackingList(this ILogger<InactivityTrackingService> logger, ulong guildId);
}

file static class InactivityTrackingOptionsDefaults
{
    public static ImmutableArray<IInactivityTracker> Trackers { get; } = ImmutableArray.Create<IInactivityTracker>(
        new UsersInactivityTracker(UsersInactivityTrackerOptions.Default),
        new IdleInactivityTracker(IdleInactivityTrackerOptions.Default));
}