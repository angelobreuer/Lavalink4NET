namespace Lavalink4NET.InactivityTracking.Trackers;

using System.Collections.Immutable;
using Lavalink4NET.Tracking;

public readonly record struct PlayerTrackingState(
    PlayerTrackingStatus Status,
    ImmutableArray<PlayerTrackerInformation> Trackers);