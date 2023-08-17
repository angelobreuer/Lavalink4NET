namespace Lavalink4NET.InactivityTracking.Trackers;

using Lavalink4NET.Tracking;

public readonly record struct PlayerTrackerInformation(
    IInactivityTracker Tracker,
    PlayerTrackingStatus Status,
    DateTimeOffset? TrackedSince = null,
    TimeSpan? Timeout = null)
{
    public DateTimeOffset? ExpiresAt => TrackedSince is not null && Timeout is not null
        ? TrackedSince.Value + Timeout.Value
        : null;
}