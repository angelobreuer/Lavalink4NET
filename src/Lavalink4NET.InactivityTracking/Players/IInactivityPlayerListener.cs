namespace Lavalink4NET.InactivityTracking.Players;

using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;

public interface IInactivityPlayerListener : ILavalinkPlayerListener
{
    ValueTask NotifyPlayerInactiveAsync(
        PlayerTrackingState trackingState,
        PlayerTrackingState previousTrackingState,
        IInactivityTracker inactivityTracker,
        CancellationToken cancellationToken = default);

    ValueTask NotifyPlayerActiveAsync(
        PlayerTrackingState trackingState,
        PlayerTrackingState previousTrackingState,
        CancellationToken cancellationToken = default);

    ValueTask NotifyPlayerTrackedAsync(
        PlayerTrackingState trackingState,
        PlayerTrackingState previousTrackingState,
        CancellationToken cancellationToken = default);
}
