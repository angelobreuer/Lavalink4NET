namespace Lavalink4NET.InactivityTracking.Trackers;

using System;

public readonly record struct IdleInactivityTrackerOptions(TimeSpan? Timeout = null)
{
    public static IdleInactivityTrackerOptions Default { get; } = new();
}
