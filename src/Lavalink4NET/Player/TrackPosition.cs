namespace Lavalink4NET.Player;

using System;

public readonly struct TrackPosition
{
    public TrackPosition(DateTimeOffset syncedAt, TimeSpan relativePosition, float timeStretchFactor = 1.0F)
    {
        SyncedAt = syncedAt;
        RelativePosition = relativePosition;
        TimeStretchFactor = timeStretchFactor;
    }

    /// <summary>
    ///     Gets the UTC time when the track position was last synced at.
    /// </summary>
    /// <value>the UTC time when the track position was last synced at.</value>
    public DateTimeOffset SyncedAt { get; }

    /// <summary>
    ///     Gets the current stretched track position relative to <see cref="SyncedAt"/>.
    /// </summary>
    /// <value>the current stretched track position relative to <see cref="SyncedAt"/>.</value>
    public TimeSpan RelativePosition { get; }

    public TimeSpan UnstretchedUnsyncedDuration => DateTimeOffset.UtcNow - SyncedAt;

    public TimeSpan UnsyncedDuration => TimeSpan.FromTicks((long)(UnstretchedUnsyncedDuration.Ticks * TimeStretchFactor));

    /// <summary>
    ///     Gets the current stretched track position.
    /// </summary>
    /// <value>the current stretched track position.</value>
    public TimeSpan Position => RelativePosition + UnsyncedDuration;

    public float TimeStretchFactor { get; }

    public bool IsStretched => TimeStretchFactor is not 1F;

    public readonly TrackPosition FixAndStretch(DateTimeOffset timestamp, float timeStretchFactor)
    {
        if (SyncedAt > timestamp)
        {
            throw new ArgumentException("Timestamp must be bigger than the current sync time.", nameof(timestamp));
        }

        // store state of the track position
        var unstretchedUnsyncedTime = timestamp - SyncedAt;
        var unsyncedDuration = TimeSpan.FromTicks((long)(unstretchedUnsyncedTime.Ticks * TimeStretchFactor));

        return new TrackPosition(timestamp, unsyncedDuration + RelativePosition, timeStretchFactor);
    }
}
