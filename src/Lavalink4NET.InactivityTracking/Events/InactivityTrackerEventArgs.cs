namespace Lavalink4NET.InactivityTracking.Events;

using System;
using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;

public abstract class InactivityTrackerEventArgs : InactivityPlayerEventArgs
{
    protected InactivityTrackerEventArgs(ILavalinkPlayer player, PlayerTrackingState trackingState, IInactivityTracker inactivityTracker)
        : base(player, trackingState)
    {
        ArgumentNullException.ThrowIfNull(inactivityTracker);

        InactivityTracker = inactivityTracker;
    }

    public IInactivityTracker InactivityTracker { get; }
}
