namespace Lavalink4NET.InactivityTracking.Trackers.Idle;

using System.Collections.Immutable;
using System.Diagnostics;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players;
using Microsoft.Extensions.Options;

public sealed class IdleInactivityTracker : IInactivityTracker
{
    private readonly string _label;
    private readonly IdleInactivityTrackerOptions _options;
    private readonly IPlayerManager _playerManager;

    public IdleInactivityTracker(
        IPlayerManager playerManager,
        IOptions<IdleInactivityTrackerOptions>? options = null)
    {
        ArgumentNullException.ThrowIfNull(playerManager);

        _label = options?.Value.Label ?? "Idle Inactivity Tracker";
        _options = options?.Value ?? new();
        _playerManager = playerManager;
    }

    public InactivityTrackerOptions Options => InactivityTrackerOptions.Realtime(_label);

    public ValueTask RunAsync(IInactivityTrackerContext trackerContext, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(trackerContext);

        var context = new IdleInactivityTrackerContext(
            trackerContext: trackerContext,
            playerManager: _playerManager,
            idleStates: _options.IdleStates,
            timeout: _options.Timeout,
            initialTimeout: _options.InitialTimeout,
            trackNewPlayers: _options.TrackNewPlayers);

        return context.RunAsync(cancellationToken);
    }
}

file sealed class IdleInactivityTrackerContext
{
    private readonly IInactivityTrackerContext _trackerContext;
    private readonly object _trackerContextSyncRoot;
    private readonly IPlayerManager _playerManager;
    private readonly ImmutableArray<PlayerState> _idleStates;
    private readonly TimeSpan? _timeout;
    private readonly TimeSpan? _initialTimeout;
    private readonly bool _trackNewPlayers;

    public IdleInactivityTrackerContext(
        IInactivityTrackerContext trackerContext,
        IPlayerManager playerManager,
        ImmutableArray<PlayerState> idleStates,
        TimeSpan? timeout,
        TimeSpan? initialTimeout,
        bool trackNewPlayers)
    {
        ArgumentNullException.ThrowIfNull(trackerContext);
        ArgumentNullException.ThrowIfNull(playerManager);

        _trackerContext = trackerContext;
        _trackerContextSyncRoot = new object();
        _playerManager = playerManager;
        _timeout = timeout;
        _idleStates = idleStates;
        _initialTimeout = initialTimeout;
        _trackNewPlayers = trackNewPlayers;
    }

    public async ValueTask RunAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _playerManager.PlayerStateChanged += PlayerStateChanged;

        if (_trackNewPlayers)
        {
            _playerManager.PlayerCreated += PlayerCreated;
        }

        try
        {
            var taskCompletionSource = new TaskCompletionSource<object?>(
                creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);

            using var cancellationTokenRegistration = cancellationToken.Register(
                state: taskCompletionSource,
                callback: taskCompletionSource.SetResult);

            await taskCompletionSource.Task.ConfigureAwait(false);
        }
        finally
        {
            _playerManager.PlayerStateChanged -= PlayerStateChanged;

            if (_trackNewPlayers)
            {
                _playerManager.PlayerCreated -= PlayerCreated;
            }
        }
    }

    private Task PlayerCreated(object sender, PlayerCreatedEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(eventArgs);

        Debug.Assert(_trackNewPlayers);

        NotifyActivityStateChange(eventArgs.Player, eventArgs.Player.State, _initialTimeout ?? _timeout);

        return Task.CompletedTask;
    }

    private Task PlayerStateChanged(object sender, PlayerStateChangedEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(eventArgs);

        if (eventArgs.Player.State is PlayerState.Destroyed)
        {
            return Task.CompletedTask; // ignore, player is destroyed
        }

        NotifyActivityStateChange(eventArgs.Player, eventArgs.State, _timeout);

        return Task.CompletedTask;
    }

    private void NotifyActivityStateChange(ILavalinkPlayer player, PlayerState playerState, TimeSpan? timeout)
    {
        lock (_trackerContextSyncRoot)
        {
            using var scope = _trackerContext.CreateScope();
            var isIdle = _idleStates.Contains(playerState);

            if (isIdle)
            {
                scope.MarkInactive(player.GuildId, timeout);
            }
            else
            {
                scope.MarkActive(player.GuildId);
            }
        }
    }
}