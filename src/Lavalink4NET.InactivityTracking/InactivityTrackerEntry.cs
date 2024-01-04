namespace Lavalink4NET.InactivityTracking;

using System;

public readonly struct InactivityTrackerEntry(DateTimeOffset inactiveSince, TimeSpan? timeout = null)
{
    private readonly long _timeoutValue = timeout.HasValue
        ? timeout.Value.Ticks
        : -1;

    public TimeSpan? Timeout => _timeoutValue < 0
        ? null
        : TimeSpan.FromTicks(_timeoutValue);

    public DateTimeOffset InactiveSince { get; } = inactiveSince;
}
