namespace Lavalink4NET.InactivityTracking.Players;

using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;

public interface IInactivityTrackerPlayerListener : ILavalinkPlayerListener
{
    ValueTask NotifyPlayerTrackerInactiveAsync(
        PlayerTrackingState trackingState,
        IInactivityTracker inactivityTracker,
        CancellationToken cancellationToken = default);

    ValueTask NotifyPlayerTrackerActiveAsync(
        PlayerTrackingState trackingState,
        IInactivityTracker inactivityTracker,
        CancellationToken cancellationToken = default);
}
