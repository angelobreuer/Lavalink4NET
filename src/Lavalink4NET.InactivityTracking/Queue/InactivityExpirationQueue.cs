namespace Lavalink4NET.InactivityTracking.Queue;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

internal sealed class InactivityExpirationQueue : IInactivityExpirationQueue
{
    private readonly ConcurrentDictionary<ulong, DateTimeOffset> _immunityMap;
    private readonly PriorityQueue<ILavalinkPlayer, DateTimeOffset> _expirationQueue;
    private readonly Queue<ExpiringPlayer> _playerQueue;
    private readonly object _playerQueueSyncRoot;
    private readonly SemaphoreSlim _queueWaitSemaphoreSlim;
    private readonly ISystemClock _systemClock;
    private readonly ILogger<InactivityExpirationQueue> _logger;
    private TaskCompletionSource? _expirationUndercutTaskCompletionSource;
    private DateTimeOffset _lowestExpirationAt;
    private int _waitState;

    public InactivityExpirationQueue(ISystemClock systemClock, ILogger<InactivityExpirationQueue> logger)
    {
        ArgumentNullException.ThrowIfNull(systemClock);
        ArgumentNullException.ThrowIfNull(logger);

        _systemClock = systemClock;
        _logger = logger;
        _immunityMap = new ConcurrentDictionary<ulong, DateTimeOffset>();
        _expirationQueue = new PriorityQueue<ILavalinkPlayer, DateTimeOffset>();
        _playerQueue = new Queue<ExpiringPlayer>();
        _playerQueueSyncRoot = new object();
        _expirationUndercutTaskCompletionSource = new TaskCompletionSource(creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);
        _queueWaitSemaphoreSlim = new SemaphoreSlim(0, int.MaxValue);
    }

    private void PromoteInternal()
    {
        // Promote pending players to the expiration queue
        _expirationQueue.EnsureCapacity(_expirationQueue.Count + _playerQueue.Count);
        Diagnostics.PromotedPlayers.Add(_playerQueue.Count);

        while (_playerQueue.TryDequeue(out var expiringPlayer))
        {
            Diagnostics.InactivityExpirationQueueSize.Add(delta: -1, tag: KeyValuePair.Create<string, object?>("label", expiringPlayer.Player.Label));
            Diagnostics.PromotedExpirationQueueSize.Add(delta: 1, tag: KeyValuePair.Create<string, object?>("label", expiringPlayer.Player.Label));

            _logger.LogInformation(
                "Player '{Label}' was promoted to the expiration queue (expires after {ExpireAfter}).",
                expiringPlayer.Player.Label, expiringPlayer.ExpireAfter);

            _expirationQueue.Enqueue(expiringPlayer.Player, expiringPlayer.ExpireAfter);
        }
    }

    public bool TryCancel(ILavalinkPlayer player)
    {
        ArgumentNullException.ThrowIfNull(player);

        if (_immunityMap.TryRemove(player.GuildId, out var expireAfter))
        {
            _logger.LogInformation(
                "Player expiration for player '{Label}' with expiration after {ExpirationAfter} was cancelled.",
                player.Label, expireAfter);

            return true;
        }

        return false;
    }

    private void NotifyInternal(ILavalinkPlayer player, DateTimeOffset expireAfter)
    {
        var isEmpty = _playerQueue.Count is 0;

        _immunityMap[player.GuildId] = expireAfter;
        _playerQueue.Enqueue(new ExpiringPlayer(player, expireAfter));

        if (isEmpty)
        {
            _queueWaitSemaphoreSlim.Release();
        }

        _logger.LogInformation(
            "Player '{Label}' will be expired after {ExpireAfter}.",
            player.Label, expireAfter);

        Diagnostics.InactivityExpirationQueueSize.Add(delta: 1, tag: KeyValuePair.Create<string, object?>("label", player.Label));
        Diagnostics.TotalExpirationQueueSize.Add(delta: 1, tag: KeyValuePair.Create<string, object?>("label", player.Label));

        if (expireAfter < _lowestExpirationAt)
        {
            _lowestExpirationAt = expireAfter;
            _expirationUndercutTaskCompletionSource?.TrySetResult();
        }
    }

    public bool TryNotify(ILavalinkPlayer player, DateTimeOffset expireAfter)
    {
        ArgumentNullException.ThrowIfNull(player);

        lock (_playerQueueSyncRoot)
        {
            NotifyInternal(player, expireAfter);
        }

        return true;
    }

    public async ValueTask<ILavalinkPlayer?> GetExpiredPlayerAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Interlocked.CompareExchange(ref _waitState, WaitState.Waiting, WaitState.None) is not WaitState.None)
        {
            throw new InvalidOperationException("Only one operation can be active at a time.");
        }

        bool Dequeue([MaybeNullWhen(false)] out ILavalinkPlayer player, [MaybeNullWhen(false)] out DateTimeOffset expiresAt)
        {
            if (!_expirationQueue.TryDequeue(out player, out expiresAt))
            {
                _logger.LogTrace("No player available in the expiration queue, waiting...");

                return false;
            }

            Diagnostics.PromotedExpirationQueueSize.Add(delta: -1, tag: KeyValuePair.Create<string, object?>("label", player.Label));
            Diagnostics.TotalExpirationQueueSize.Add(delta: -1, tag: KeyValuePair.Create<string, object?>("label", player.Label));

            while (PeekImmune(player, expiresAt))
            {
                IsImmune(player, expiresAt);

                _logger.LogTrace(
                    "The expiration of the promoted player '{Label}' was cancelled.",
                    player.Label);

                if (!_expirationQueue.TryDequeue(out player, out expiresAt))
                {
                    _logger.LogTrace("No player available in the expiration queue, waiting...");

                    return false;
                }

                Diagnostics.PromotedExpirationQueueSize.Add(delta: -1, tag: KeyValuePair.Create<string, object?>("label", player.Label));
                Diagnostics.TotalExpirationQueueSize.Add(delta: -1, tag: KeyValuePair.Create<string, object?>("label", player.Label));
            }

            _logger.LogTrace(
                "Player '{Label}' was dequeued from the expiration queue (expires after {ExpireAfter}).",
                       player.Label, expiresAt);

            return true;
        }

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var taskCompletionSource = new TaskCompletionSource(
                    creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);

                var previousTaskCompletionSource = Interlocked.Exchange(
                    location1: ref _expirationUndercutTaskCompletionSource,
                    value: taskCompletionSource);

                previousTaskCompletionSource?.TrySetCanceled(CancellationToken.None);

                var utcNow = _systemClock.UtcNow;

                ILavalinkPlayer? player;
                DateTimeOffset expiresAt;
                while (!Dequeue(out player, out expiresAt))
                {
                    await _queueWaitSemaphoreSlim
                        .WaitAsync(cancellationToken)
                        .ConfigureAwait(false);

                    PromoteInternal();

                    utcNow = _systemClock.UtcNow;
                }

                var waitDuration = expiresAt - utcNow;

                if (waitDuration > TimeSpan.FromMilliseconds(1))
                {
                    var undercutTask = taskCompletionSource.Task;
                    var waitTask = Task.Delay(waitDuration, cancellationToken);

                    var task = await Task
                        .WhenAny(undercutTask, waitTask)
                        .ConfigureAwait(false);

                    if (ReferenceEquals(task, undercutTask))
                    {
                        _logger.LogTrace(
                            "Player '{Label}' was undercut by a new player with expiration after {ExpireAfter}.",
                            player.Label, expiresAt);

                        PromoteInternal();
                        continue;
                    }
                }

                if (!IsImmune(player, expiresAt))
                {
                    _logger.LogInformation(
                        "Player '{Label}' expired after {ExpireAfter}.",
                        player.Label, expiresAt);

                    Diagnostics.ExpiredPlayers.Add(1);

                    return player;
                }
                else
                {
                    _logger.LogTrace(
                        "The expiration of the promoted player '{Label}' was cancelled.",
                        player.Label);

                    Diagnostics.CancelledExpirationPlayers.Add(1);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // returns null
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while processing inactivity expiration queue.");
            throw;
        }
        finally
        {
            Volatile.Write(ref _waitState, WaitState.None);
        }

        return null;
    }

    private bool IsImmune(ILavalinkPlayer player, DateTimeOffset expireAfter)
    {
        return !_immunityMap.TryRemove(KeyValuePair.Create(player.GuildId, expireAfter));
    }

    private bool PeekImmune(ILavalinkPlayer player, DateTimeOffset expireAfter)
    {
        return !_immunityMap.TryGetValue(player.GuildId, out var expireAfterValue)
            || expireAfterValue != expireAfter;
    }
}

file static class WaitState
{
    public const int None = 0;
    public const int Waiting = 1;
}

internal readonly record struct ExpiringPlayer(ILavalinkPlayer Player, DateTimeOffset ExpireAfter);

file static class Diagnostics
{
    static Diagnostics()
    {
        var meter = new Meter("Lavalink4NET");

        InactivityExpirationQueueSize = meter.CreateUpDownCounter<int>("inactivity-expiration-queue-size", "Players");
        PromotedExpirationQueueSize = meter.CreateUpDownCounter<int>("inactivity-promoted-expiration-queue-size", "Players");
        TotalExpirationQueueSize = meter.CreateUpDownCounter<int>("inactivity-total-expiration-queue-size", "Players");
        CancelledExpirationPlayers = meter.CreateCounter<int>("inactivity-cancelled-expiration-players", "Players");
        ExpiredPlayers = meter.CreateCounter<int>("inactivity-expired-players", "Players");
        PromotedPlayers = meter.CreateCounter<int>("inactivity-promoted-players", "Players");
    }

    public static UpDownCounter<int> InactivityExpirationQueueSize { get; }

    public static UpDownCounter<int> PromotedExpirationQueueSize { get; }

    public static UpDownCounter<int> TotalExpirationQueueSize { get; }

    public static Counter<int> CancelledExpirationPlayers { get; }

    public static Counter<int> ExpiredPlayers { get; }

    public static Counter<int> PromotedPlayers { get; }
}