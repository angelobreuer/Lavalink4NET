namespace Lavalink4NET.InactivityTracking.Events;

using System;
using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;

public abstract class InactivityPlayerEventArgs : EventArgs
{
    protected InactivityPlayerEventArgs(ILavalinkPlayer player, PlayerTrackingState trackingState)
    {
        ArgumentNullException.ThrowIfNull(player);

        Player = player;
        TrackingState = trackingState;
    }

    public ILavalinkPlayer Player { get; }

    public PlayerTrackingState TrackingState { get; }
}
