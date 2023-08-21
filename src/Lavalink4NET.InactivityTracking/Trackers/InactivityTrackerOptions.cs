namespace Lavalink4NET.InactivityTracking.Trackers;

using System;

public readonly struct InactivityTrackerOptions
{
    private InactivityTrackerOptions(string? label, TimeSpan? pollInterval, InactivityTrackerMode mode)
    {
        Label = label;
        PollInterval = pollInterval;
        Mode = mode;
    }

    public static InactivityTrackerOptions Default => default;

    public static InactivityTrackerOptions Polling(
        string? label = null,
        TimeSpan? pollInterval = null)
    {
        return new(label, pollInterval, InactivityTrackerMode.Polling);
    }

    public static InactivityTrackerOptions Realtime(string? label = null)
    {
        return new(
            label: label,
            pollInterval: null,
            mode: InactivityTrackerMode.Realtime);
    }

    public string? Label { get; }

    public TimeSpan? PollInterval { get; }

    public InactivityTrackerMode Mode { get; }
}

public enum InactivityTrackerMode : byte
{
    Polling,
    Realtime,
}
