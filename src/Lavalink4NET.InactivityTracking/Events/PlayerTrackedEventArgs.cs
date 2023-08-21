namespace Lavalink4NET.InactivityTracking.Events;

using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;

public sealed class PlayerTrackedEventArgs(ILavalinkPlayer player, PlayerTrackingState trackingState)
    : InactivityPlayerEventArgs(player, trackingState)
{
}
