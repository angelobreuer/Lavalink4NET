namespace Lavalink4NET.InactivityTracking;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using Lavalink4NET.Clients;
using Lavalink4NET.InactivityTracking.Events;
using Lavalink4NET.InactivityTracking.Players;
using Lavalink4NET.InactivityTracking.Queue;
using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.InactivityTracking.Trackers.Idle;
using Lavalink4NET.InactivityTracking.Trackers.Lifetime;
using Lavalink4NET.InactivityTracking.Trackers.Users;
using Lavalink4NET.Players;
using Lavalink4NET.Tracking;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
///     A service that tracks not-playing players to reduce the usage of the Lavalink nodes.
/// </summary>
public partial class InactivityTrackingService : IInactivityTrackingService
{
    private readonly IInactivityExpirationQueue _expirationQueue;
    private readonly IPlayerManager _playerManager;
    private readonly ISystemClock _systemClock;
    private readonly ImmutableArray<IInactivityTrackerLifetime> _lifetimes;
    private readonly Dictionary<ulong, PlayerTrackerMap> _trackingMap;
    private readonly SemaphoreSlim _playersSemaphoreSlim;
    private readonly PlayerTrackingState _defaultTrackingState;
    private readonly Channel<IEventDispatch> _eventQueueChannel;
    private readonly InactivityTrackingMode _trackingMode;
    private readonly InactivityTrackingTimeoutBehavior _timeoutBehavior;
    private readonly TimeSpan _defaultTimeout;
    private readonly TimeSpan _defaultPollInterval;
    private readonly uint _allMask;
    private CancellationTokenSource? _stoppingCancellationTokenSource;
    private bool _disposed;
    private Task? _executeTask;

    public InactivityTrackingService(
        IPlayerManager playerManager,
        IInactivityExpirationQueue expirationQueue,
        IDiscordClientWrapper discordClient,
        ISystemClock systemClock,
        IEnumerable<IInactivityTracker> trackers,
        IOptions<InactivityTrackingOptions> options,
        ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(expirationQueue);
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(systemClock);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        IInactivityTrackerLifetime CreateLifetime(IInactivityTracker inactivityTracker)
        {
            ArgumentNullException.ThrowIfNull(inactivityTracker);

            var trackerOptions = inactivityTracker.Options;
            var label = trackerOptions.Label ?? inactivityTracker.GetType().Name;

            var inactivityTrackerContext = new InactivityTrackerContext(
                inactivityTrackingService: this,
                inactivityTracker: inactivityTracker,
                systemClock: systemClock);

            if (trackerOptions.Mode is InactivityTrackerMode.Polling)
            {
                var pollInterval = trackerOptions.PollInterval ?? _defaultPollInterval;

                return new PollingInactivityTrackerLifetime(
                    label,
                    inactivityTracker: inactivityTracker,
                    inactivityTrackerContext: inactivityTrackerContext,
                    logger: loggerFactory.CreateLogger<PollingInactivityTrackerLifetime>(),
                    pollInterval: pollInterval);
            }
            else
            {
                return new RealtimeInactivityTrackerLifetime(
                    label,
                    inactivityTracker: inactivityTracker,
                    inactivityTrackerContext: inactivityTrackerContext,
                    logger: loggerFactory.CreateLogger<RealtimeInactivityTrackerLifetime>());
            }
        }

        var eventQueueChannelOptions = new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false,
        };

        _eventQueueChannel = Channel.CreateUnbounded<IEventDispatch>(eventQueueChannelOptions);
        _trackingMap = new Dictionary<ulong, PlayerTrackerMap>();
        _stoppingCancellationTokenSource = new CancellationTokenSource();

        var trackersArray = options.Value.Trackers is null
             ? trackers.ToImmutableArray()
             : options.Value.Trackers.Value;

        if (trackersArray.IsDefaultOrEmpty && options.Value.UseDefaultTrackers)
        {
            trackersArray = ImmutableArray.Create<IInactivityTracker>(
                item1: new UsersInactivityTracker(discordClient, playerManager),
                item2: new IdleInactivityTracker(playerManager));
        }

        if (trackersArray.Length > 32)
        {
            throw new InvalidOperationException("The maximum number of trackers is 32.");
        }

        _lifetimes = trackersArray
            .Select(CreateLifetime)
            .ToImmutableArray();

        _playerManager = playerManager;
        _expirationQueue = expirationQueue;
        _systemClock = systemClock;

        _defaultTimeout = options.Value.DefaultTimeout;
        _defaultPollInterval = options.Value.DefaultPollInterval;
        _trackingMode = options.Value.TrackingMode;
        _timeoutBehavior = options.Value.TimeoutBehavior;

        Logger = loggerFactory.CreateLogger<InactivityTrackingService>();

        PausedPlayers = options.Value.InactivityBehavior is PlayerInactivityBehavior.Pause
            ? new PausedPlayersState(new object(), new HashSet<ulong>())
            : null;

        _playersSemaphoreSlim = new SemaphoreSlim(1, 1);
        _defaultTrackingState = CreateDefaultTrackingState(trackersArray);

        // mask used to detect when all players are active, used when the tracking mode is "All"
        _allMask = trackersArray.Length is 32
            ? uint.MaxValue
            : (uint)(1 << trackersArray.Length) - 1;
    }

    internal PausedPlayersState? PausedPlayers { get; }

    internal ILogger<InactivityTrackingService> Logger { get; }

    public void Report(
        IInactivityTracker inactivityTracker,
        IImmutableSet<ulong> activePlayers,
        IImmutableDictionary<ulong, InactivityTrackerEntry> trackedPlayers)
    {
        ArgumentNullException.ThrowIfNull(inactivityTracker);
        ArgumentNullException.ThrowIfNull(trackedPlayers);
        ArgumentNullException.ThrowIfNull(activePlayers);

        int GetTrackerIndex(IInactivityTracker tracker)
        {
            for (var trackerIndex = 0; trackerIndex < _lifetimes.Length; trackerIndex++)
            {
                if (_lifetimes[trackerIndex].InactivityTracker == tracker)
                {
                    return trackerIndex;
                }
            }

            throw new InvalidOperationException("The tracker is not registered.");
        }

        var trackerIndex = GetTrackerIndex(inactivityTracker);

        _playersSemaphoreSlim.Wait();

        try
        {
            ReportInternal(activePlayers, trackedPlayers, trackerIndex);
        }
        finally
        {
            _playersSemaphoreSlim.Release();
        }
    }

    private void ReportInternal(IImmutableSet<ulong> activePlayers, IImmutableDictionary<ulong, InactivityTrackerEntry> trackedPlayers, int trackerIndex)
    {
        Debug.Assert(!activePlayers.Any(trackedPlayers.ContainsKey));

        var utcNow = _systemClock.UtcNow;
        var lifetime = _lifetimes[trackerIndex];
        var mask = 1U << trackerIndex;
        var inverseMask = ~mask;

        TimeSpan? ComputeLowestTimeout(PlayerTrackerMap entryMap)
        {
            var lowestTimeout = default(TimeSpan?);

            for (var trackerIndex = 0; trackerIndex < _lifetimes.Length; trackerIndex++)
            {
                if ((entryMap.Bits & (1 << trackerIndex)) is 0)
                {
                    continue;
                }

                var entry = entryMap.Entries.First(
                     x => x.TrackerIndex == trackerIndex);

                if (lowestTimeout is null || entry.Timeout < lowestTimeout)
                {
                    lowestTimeout = entry.Timeout;
                }
            }

            return lowestTimeout;
        }

        TimeSpan? ComputeHighestTimeout(PlayerTrackerMap entryMap)
        {
            var highestTimeout = default(TimeSpan?);

            for (var trackerIndex = 0; trackerIndex < _lifetimes.Length; trackerIndex++)
            {
                if ((entryMap.Bits & (1 << trackerIndex)) is 0)
                {
                    continue;
                }

                var entry = entryMap.Entries.First(
                     x => x.TrackerIndex == trackerIndex);

                if (highestTimeout is null || entry.Timeout > highestTimeout)
                {
                    highestTimeout = entry.Timeout;
                }
            }

            return highestTimeout;
        }

        TimeSpan? ComputeAverageTimeout(PlayerTrackerMap entryMap)
        {
            var activeTrackers = 0;
            var averageTimeout = TimeSpan.Zero;

            for (var trackerIndex = 0; trackerIndex < _lifetimes.Length; trackerIndex++)
            {
                if ((entryMap.Bits & (1 << trackerIndex)) is 0)
                {
                    continue;
                }

                var entry = entryMap.Entries.First(
                     x => x.TrackerIndex == trackerIndex);

                averageTimeout += entry.Timeout;
                activeTrackers++;
            }

            if (activeTrackers is 0)
            {
                return null;
            }

            return averageTimeout / activeTrackers;
        }

        TimeSpan? ComputeTimeout(PlayerTrackerMap entryMap) => _timeoutBehavior switch
        {
            InactivityTrackingTimeoutBehavior.Lowest => ComputeLowestTimeout(entryMap),
            InactivityTrackingTimeoutBehavior.Highest => ComputeHighestTimeout(entryMap),
            InactivityTrackingTimeoutBehavior.Average => ComputeAverageTimeout(entryMap),
            _ => throw new NotSupportedException(),
        };

        void UpdateTimeout(ILavalinkPlayer player, in PlayerTrackerMap entryMap)
        {
            var timeout = _trackingMode is not InactivityTrackingMode.All || entryMap.Bits == _allMask
                ? ComputeTimeout(entryMap)
                : default;

            _expirationQueue.TryCancel(player);

            if (timeout is not null)
            {
                _expirationQueue.TryNotify(player, utcNow + timeout.Value);
            }
        }

        void ActivatePlayer(ILavalinkPlayer player)
        {
            ref var trackerMap = ref CollectionsMarshal.GetValueRefOrNullRef(_trackingMap, player.GuildId);

            if (Unsafe.IsNullRef(ref trackerMap))
            {
                return;
            }

            Debug.Assert(trackerMap.Entries.Any(x => x.TrackerIndex == trackerIndex));
            Debug.Assert((trackerMap.Bits & mask) is not 0);

            trackerMap.Bits &= inverseMask;
            trackerMap.Entries = trackerMap.Entries.RemoveAll(x => x.TrackerIndex == trackerIndex);

            Dispatch(new TrackerActiveEventDispatch(player, lifetime.InactivityTracker));

            if (trackerMap.Bits is 0)
            {
                // All bits cleared, player is active again
                Dispatch(new PlayerActiveEventDispatch(player));

                // Clear the entry from the tracker map since no trackers are tracking it anymore
                var result = _trackingMap.Remove(player.GuildId);
                Debug.Assert(result);
            }

            UpdateTimeout(player, trackerMap);
        }

        void UpdateTrackerEntry(ref PlayerTrackerMap trackerMap, InactivityTrackerEntry presentEntry)
        {
            for (var index = 0; index < trackerMap.Entries.Length; index++)
            {
                if (trackerMap.Entries[index].TrackerIndex == trackerIndex)
                {
                    var updatedEntry = new PlayerTrackerMapEntry(
                        TrackerIndex: trackerIndex,
                        InactiveSince: trackerMap.Entries[index].InactiveSince,
                        Timeout: presentEntry.Timeout ?? _defaultTimeout);

                    trackerMap.Entries = trackerMap.Entries.SetItem(
                        index,
                        item: updatedEntry);

                    return;
                }
            }

            Debug.Assert(false);
        }

        void MarkPlayerInactive(ILavalinkPlayer player, InactivityTrackerEntry entry)
        {
            ref var trackerMap = ref CollectionsMarshal.GetValueRefOrAddDefault(_trackingMap, player.GuildId, out var exists);

            if (!exists)
            {
                // Player is now tracked initially
                trackerMap.Entries = ImmutableArray.Create(new PlayerTrackerMapEntry(
                    TrackerIndex: trackerIndex,
                    InactiveSince: entry.InactiveSince,
                    Timeout: entry.Timeout ?? _defaultTimeout));

                trackerMap.Bits = mask;

                Dispatch(new PlayerTrackedEventDispatch(player));
                Dispatch(new TrackerInactiveEventDispatch(player, lifetime.InactivityTracker));
            }
            else if ((trackerMap.Bits & mask) is not 0)
            {
                // Player is already tracked by this tracker
                UpdateTrackerEntry(ref trackerMap, entry);
            }
            else
            {
                // Player is initially tracked by this tracker but was marked inactive by another tracker
                trackerMap.Bits |= mask;

                trackerMap.Entries = trackerMap.Entries.Add(new PlayerTrackerMapEntry(
                    TrackerIndex: trackerIndex,
                    InactiveSince: entry.InactiveSince,
                    Timeout: entry.Timeout ?? _defaultTimeout));

                Dispatch(new TrackerInactiveEventDispatch(player, lifetime.InactivityTracker));
            }

            UpdateTimeout(player, trackerMap);
        }

        foreach (var activePlayerId in activePlayers)
        {
            if (!_playerManager.TryGetPlayer(activePlayerId, out var player))
            {
                continue;
            }

            Logger.PlayerMarkedActive(lifetime.Label, player.Label);
            ActivatePlayer(player);
        }

        foreach (var (trackedPlayerId, entry) in trackedPlayers)
        {
            if (!_playerManager.TryGetPlayer(trackedPlayerId, out var player))
            {
                continue;
            }

            MarkPlayerInactive(player, entry);

            var expireAfter = entry.InactiveSince + (entry.Timeout ?? _defaultTimeout);
            Logger.PlayerMarkedInactive(lifetime.Label, player.Label, expireAfter);
        }
    }

    private void Dispatch(IEventDispatch eventDispatch)
    {
        _eventQueueChannel.Writer.TryWrite(eventDispatch);
    }

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
            return _trackingMap.TryGetValue(player.GuildId, out var trackingMap)
                ? CreateTrackingState(trackingMap)
                : _defaultTrackingState;
        }
        finally
        {
            _playersSemaphoreSlim.Release();
        }
    }

    internal PlayerTrackingState GetPlayer(ILavalinkPlayer player)
    {
        ThrowIfDisposed();

        _playersSemaphoreSlim.Wait();

        try
        {
            return _trackingMap.TryGetValue(player.GuildId, out var trackingMap)
                ? CreateTrackingState(trackingMap)
                : _defaultTrackingState;
        }
        finally
        {
            _playersSemaphoreSlim.Release();
        }
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_disposed)
        {
            return;
        }

        await StopAsync().ConfigureAwait(false);
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
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

    private PlayerTrackingState CreateTrackingState(PlayerTrackerMap entryMap)
    {
        PlayerTrackerInformation CreateTrackerInformation(IInactivityTrackerLifetime trackerLifetime, int trackerIndex)
        {
            var mask = 1 << trackerIndex;

            if ((entryMap.Bits & mask) is 0)
            {
                return new PlayerTrackerInformation(
                    Tracker: trackerLifetime.InactivityTracker,
                    Status: PlayerTrackingStatus.NotTracked,
                    TrackedSince: null,
                    Timeout: null);
            }

            var entry = entryMap.Entries.First(
                x => x.TrackerIndex == trackerIndex);

            return new PlayerTrackerInformation(
                Tracker: trackerLifetime.InactivityTracker,
                Status: PlayerTrackingStatus.Tracked,
                TrackedSince: entry.InactiveSince,
                Timeout: entry.Timeout);
        }

        Debug.Assert(entryMap.Bits is not 0);

        var trackers = _lifetimes
            .Select(CreateTrackerInformation)
            .ToImmutableArray();

        return new PlayerTrackingState(
            Status: PlayerTrackingStatus.Tracked,
            Trackers: trackers);
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

    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        _stoppingCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executeTask = RunAsync(_stoppingCancellationTokenSource.Token);

        if (_executeTask.IsCompleted)
        {
            return new ValueTask(_executeTask);
        }

        return ValueTask.CompletedTask;
    }

    public async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_executeTask == null)
        {
            return;
        }

        try
        {
            _stoppingCancellationTokenSource!.Cancel();
        }
        finally
        {
            await _executeTask
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private async Task RunAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Logger.InactivityTrackingServiceStarting();

        foreach (var lifetime in _lifetimes)
        {
            await lifetime
                .StartAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        try
        {
            Logger.InactivityTrackingServiceStarted();

            var events = _eventQueueChannel.Reader
                .ReadAllAsync(cancellationToken)
                .ConfigureAwait(false);

            await foreach (var eventDispatch in events)
            {
                await eventDispatch
                    .InvokeAsync(this, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
        finally
        {
            Logger.InactivityTrackingServiceStopping();

            foreach (var lifetime in _lifetimes)
            {
                await lifetime
                    .StopAsync(CancellationToken.None)
                    .ConfigureAwait(false);
            }

            Logger.InactivityTrackingServiceStopped();
        }
    }
}

internal static partial class Logger
{
    [LoggerMessage(1, LogLevel.Debug, "Tracked player {GuildId} as inactive.", EventName = nameof(TrackedPlayerAsInactive))]
    public static partial void TrackedPlayerAsInactive(this ILogger<InactivityTrackingService> logger, ulong guildId);

    [LoggerMessage(2, LogLevel.Debug, "Destroyed player {GuildId} due inactivity.", EventName = nameof(DestroyedPlayerDueInactivity))]
    public static partial void DestroyedPlayerDueInactivity(this ILogger<InactivityTrackingService> logger, ulong guildId);

    [LoggerMessage(3, LogLevel.Debug, "Removed player {GuildId} from tracking list.", EventName = nameof(RemovedPlayerFromTrackingList))]
    public static partial void RemovedPlayerFromTrackingList(this ILogger<InactivityTrackingService> logger, ulong guildId);

    [LoggerMessage(4, LogLevel.Information, "The inactivity tracking service is starting...", EventName = nameof(InactivityTrackingServiceStarting))]
    public static partial void InactivityTrackingServiceStarting(this ILogger<InactivityTrackingService> logger);

    [LoggerMessage(5, LogLevel.Information, "The inactivity tracking service has started.", EventName = nameof(InactivityTrackingServiceStarted))]
    public static partial void InactivityTrackingServiceStarted(this ILogger<InactivityTrackingService> logger);

    [LoggerMessage(6, LogLevel.Information, "The inactivity tracking service is stopping...", EventName = nameof(InactivityTrackingServiceStopping))]
    public static partial void InactivityTrackingServiceStopping(this ILogger<InactivityTrackingService> logger);

    [LoggerMessage(7, LogLevel.Information, "The inactivity tracking service has stopped.", EventName = nameof(InactivityTrackingServiceStopped))]
    public static partial void InactivityTrackingServiceStopped(this ILogger<InactivityTrackingService> logger);

    [LoggerMessage(8, LogLevel.Debug, "Tracker '{Tracker}' reported the following player(s) as inactive: '{Label}' (expires after: {ExpireAfter}).", EventName = nameof(PlayerMarkedInactive))]
    public static partial void PlayerMarkedInactive(this ILogger<InactivityTrackingService> logger, string tracker, string label, DateTimeOffset expireAfter);

    [LoggerMessage(9, LogLevel.Debug, "Tracker '{Tracker}' reported the following player as active: '{Label}'.", EventName = nameof(PlayerMarkedActive))]
    public static partial void PlayerMarkedActive(this ILogger<InactivityTrackingService> logger, string tracker, string label);

    [LoggerMessage(10, LogLevel.Debug, "Player '{Label}' was paused by the inactivity tracking service.", EventName = nameof(PlayerPausedByInactivityTrackingService))]
    public static partial void PlayerPausedByInactivityTrackingService(this ILogger<InactivityTrackingService> logger, string label);

    [LoggerMessage(11, LogLevel.Debug, "Player '{Label}' was not paused by the inactivity tracking service as it is already paused.", EventName = nameof(PlayerNotPausedByInactivityTrackingServiceBecauseAlreadyPaused))]
    public static partial void PlayerNotPausedByInactivityTrackingServiceBecauseAlreadyPaused(this ILogger<InactivityTrackingService> logger, string label);

    [LoggerMessage(12, LogLevel.Debug, "Player '{Label}' was resumed by the inactivity tracking service.", EventName = nameof(PlayerResumedByInactivityTrackingService))]
    public static partial void PlayerResumedByInactivityTrackingService(this ILogger<InactivityTrackingService> logger, string label);
}

internal readonly record struct PausedPlayersState(
    object SynchronizationRoot,
    HashSet<ulong> PausedPlayers);

internal readonly record struct PlayerTrackerMapEntry(int TrackerIndex, DateTimeOffset InactiveSince, TimeSpan Timeout)
{
    private readonly int _timeoutValue = (int)Timeout.TotalMilliseconds;

    public TimeSpan Timeout => TimeSpan.FromMilliseconds(_timeoutValue);

    public DateTimeOffset InactiveSince { get; } = InactiveSince;
}

internal struct PlayerTrackerMap
{
    public uint Bits;
    public ImmutableArray<PlayerTrackerMapEntry> Entries;
}

internal interface IEventDispatch
{
    ValueTask InvokeAsync(
        InactivityTrackingService inactivityTrackingService,
        CancellationToken cancellationToken = default);
}

file sealed class PlayerActiveEventDispatch(ILavalinkPlayer Player) : IEventDispatch
{
    public async ValueTask InvokeAsync(
        InactivityTrackingService inactivityTrackingService,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var state = inactivityTrackingService.GetPlayer(Player);

        if (Player is IInactivityPlayerListener inactivityPlayerListener)
        {
            await inactivityPlayerListener
                .NotifyPlayerActiveAsync(state, cancellationToken)
                .ConfigureAwait(false);
        }

        if (inactivityTrackingService.HasPlayerActiveEventHandler)
        {
            var eventArgs = new PlayerActiveEventArgs(Player, state);

            await inactivityTrackingService
                .OnPlayerActiveAsync(eventArgs)
                .ConfigureAwait(false);
        }

        var pausedPlayers = inactivityTrackingService.PausedPlayers;

        if (pausedPlayers is not null)
        {
            bool wasPausedByService;
            lock (pausedPlayers.Value.SynchronizationRoot)
            {
                wasPausedByService = pausedPlayers.Value.PausedPlayers.Remove(Player.GuildId);
            }

            if (wasPausedByService && Player.State is PlayerState.Paused)
            {
                await Player
                    .ResumeAsync(cancellationToken)
                    .ConfigureAwait(false);

                inactivityTrackingService.Logger.PlayerResumedByInactivityTrackingService(Player.Label);
            }
        }
    }
}

file sealed class PlayerInactiveEventDispatch(ILavalinkPlayer Player) : IEventDispatch
{
    public async ValueTask InvokeAsync(
        InactivityTrackingService inactivityTrackingService,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var state = inactivityTrackingService.GetPlayer(Player);

        if (Player is IInactivityPlayerListener inactivityPlayerListener)
        {
            await inactivityPlayerListener
                .NotifyPlayerInactiveAsync(state, cancellationToken)
                .ConfigureAwait(false);
        }

        if (inactivityTrackingService.HasPlayerInactiveEventHandler)
        {
            var eventArgs = new PlayerInactiveEventArgs(Player, state);

            await inactivityTrackingService
                .OnPlayerInactiveAsync(eventArgs)
                .ConfigureAwait(false);
        }
    }
}

file sealed class PlayerTrackedEventDispatch(ILavalinkPlayer Player) : IEventDispatch
{
    public async ValueTask InvokeAsync(
        InactivityTrackingService inactivityTrackingService,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var state = inactivityTrackingService.GetPlayer(Player);

        if (Player is IInactivityPlayerListener inactivityPlayerListener)
        {
            await inactivityPlayerListener
                .NotifyPlayerTrackedAsync(state, cancellationToken)
                .ConfigureAwait(false);
        }

        if (inactivityTrackingService.HasPlayerTrackedEventHandler)
        {
            var eventArgs = new PlayerTrackedEventArgs(Player, state);

            await inactivityTrackingService
                .OnPlayerTrackedAsync(eventArgs)
                .ConfigureAwait(false);
        }

        var pausedPlayers = inactivityTrackingService.PausedPlayers;

        if (pausedPlayers is not null && Player.State is not PlayerState.Paused)
        {
            if (Player.State is not PlayerState.Paused)
            {
                await Player
                    .PauseAsync(cancellationToken)
                    .ConfigureAwait(false);

                lock (pausedPlayers.Value.SynchronizationRoot)
                {
                    pausedPlayers.Value.PausedPlayers.Add(Player.GuildId);
                }

                inactivityTrackingService.Logger.PlayerPausedByInactivityTrackingService(Player.Label);
            }
            else
            {
                inactivityTrackingService.Logger.PlayerNotPausedByInactivityTrackingServiceBecauseAlreadyPaused(Player.Label);
            }
        }
    }
}

file sealed class TrackerActiveEventDispatch(ILavalinkPlayer Player, IInactivityTracker Tracker) : IEventDispatch
{
    public async ValueTask InvokeAsync(
        InactivityTrackingService inactivityTrackingService,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var state = inactivityTrackingService.GetPlayer(Player);

        if (Tracker is IInactivityTrackerPlayerListener inactivityTrackerListener)
        {
            await inactivityTrackerListener
                .NotifyPlayerTrackerActiveAsync(state, Tracker, cancellationToken)
                .ConfigureAwait(false);
        }

        if (inactivityTrackingService.HasTrackerActiveEventHandler)
        {
            var eventArgs = new TrackerActiveEventArgs(Player, state, Tracker);

            await inactivityTrackingService
                .OnTrackerActiveAsync(eventArgs)
                .ConfigureAwait(false);
        }
    }
}

file sealed class TrackerInactiveEventDispatch(ILavalinkPlayer Player, IInactivityTracker Tracker) : IEventDispatch
{
    public async ValueTask InvokeAsync(
        InactivityTrackingService inactivityTrackingService,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var state = inactivityTrackingService.GetPlayer(Player);

        if (Tracker is IInactivityTrackerPlayerListener inactivityTrackerListener)
        {
            await inactivityTrackerListener
                .NotifyPlayerTrackerInactiveAsync(state, Tracker, cancellationToken)
                .ConfigureAwait(false);
        }

        if (inactivityTrackingService.HasTrackerInactiveEventHandler)
        {
            var eventArgs = new TrackerInactiveEventArgs(Player, state, Tracker);

            await inactivityTrackingService
                .OnTrackerInactiveAsync(eventArgs)
                .ConfigureAwait(false);
        }
    }
}
