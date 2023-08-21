namespace Lavalink4NET.InactivityTracking.Players;

using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;

public interface IInactivityPlayerListener : ILavalinkPlayerListener
{
    ValueTask NotifyPlayerInactiveAsync(
        PlayerTrackingState trackingState,
        CancellationToken cancellationToken = default);

    ValueTask NotifyPlayerActiveAsync(
        PlayerTrackingState trackingState,
        CancellationToken cancellationToken = default);

    ValueTask NotifyPlayerTrackedAsync(
        PlayerTrackingState trackingState,
        CancellationToken cancellationToken = default);
}
