namespace Lavalink4NET.InactivityTracking;

using System;

public readonly struct InactivityTrackerEntry(DateTimeOffset inactiveSince, TimeSpan? timeout = null)
{
    private readonly int _timeoutValue = timeout.HasValue
        ? (int)timeout.Value.TotalMilliseconds
        : -1;

    public TimeSpan? Timeout => _timeoutValue < 0
        ? null
        : TimeSpan.FromMilliseconds(_timeoutValue);

    public DateTimeOffset InactiveSince { get; } = inactiveSince;
}
