namespace Lavalink4NET.InactivityTracking.Trackers.Idle;

using System.Collections.Immutable;
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
             timeout: _options.Timeout);

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

    public IdleInactivityTrackerContext(
        IInactivityTrackerContext trackerContext,
        IPlayerManager playerManager,
        ImmutableArray<PlayerState> idleStates,
        TimeSpan? timeout)
    {
        ArgumentNullException.ThrowIfNull(trackerContext);
        ArgumentNullException.ThrowIfNull(playerManager);

        _trackerContext = trackerContext;
        _trackerContextSyncRoot = new object();
        _playerManager = playerManager;
        _timeout = timeout;
        _idleStates = idleStates;
    }

    public async ValueTask RunAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _playerManager.PlayerStateChanged += PlayerStateChanged;

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
        }
    }

    private Task PlayerStateChanged(object sender, PlayerStateChangedEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(eventArgs);

        lock (_trackerContextSyncRoot)
        {
            using var scope = _trackerContext.CreateScope();
            var isIdle = _idleStates.Contains(eventArgs.State);

            if (isIdle)
            {
                scope.MarkInactive(eventArgs.Player.GuildId, _timeout);
            }
            else
            {
                scope.MarkActive(eventArgs.Player.GuildId);
            }
        }

        return Task.CompletedTask;
    }
}