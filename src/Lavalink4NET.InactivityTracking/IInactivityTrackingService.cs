namespace Lavalink4NET.InactivityTracking;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Events;
using Lavalink4NET.InactivityTracking.Events;
using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;

public interface IInactivityTrackingService : IAsyncDisposable
{
    event AsyncEventHandler<PlayerInactiveEventArgs>? PlayerInactive;

    event AsyncEventHandler<PlayerActiveEventArgs>? PlayerActive;

    event AsyncEventHandler<PlayerTrackedEventArgs>? PlayerTracked;

    event AsyncEventHandler<TrackerActiveEventArgs>? TrackerActive;

    event AsyncEventHandler<TrackerInactiveEventArgs>? TrackerInactive;

    ValueTask StartAsync(CancellationToken cancellationToken = default);

    ValueTask StopAsync(CancellationToken cancellationToken = default);

    ValueTask<PlayerTrackingState> GetPlayerAsync(
        ILavalinkPlayer player,
        CancellationToken cancellationToken = default);

    void Report(
        IInactivityTracker inactivityTracker,
        IImmutableSet<ulong> activePlayers,
        IImmutableDictionary<ulong, InactivityTrackerEntry> trackedPlayers);
}