namespace Lavalink4NET.InactivityTracking.Events;

using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;

public sealed class TrackerInactiveEventArgs(ILavalinkPlayer player, PlayerTrackingState trackingState, IInactivityTracker inactivityTracker)
    : InactivityTrackerEventArgs(player, trackingState, inactivityTracker)
{
}
