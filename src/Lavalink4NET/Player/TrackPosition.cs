namespace Lavalink4NET.Player;

using System;

public readonly struct TrackPosition
{
    public TrackPosition(DateTimeOffset syncedAt, TimeSpan unstretchedRelativePosition, float timeStretchFactor = 1.0F)
    {
        SyncedAt = syncedAt;
        UnstretchedRelativePosition = unstretchedRelativePosition;
        TimeStretchFactor = timeStretchFactor;
    }

    /// <summary>
    ///     Gets the UTC time when the track position was last synced at.
    /// </summary>
    /// <value>the UTC time when the track position was last synced at.</value>
    public DateTimeOffset SyncedAt { get; }

    /// <summary>
    ///     Gets the current unstretched track position relative to <see cref="SyncedAt"/>.
    /// </summary>
    /// <value>the current unstretched track position relative to <see cref="SyncedAt"/>.</value>
    public TimeSpan UnstretchedRelativePosition { get; }

    /// <summary>
    ///     Gets the current stretched track position relative to <see cref="SyncedAt"/>.
    /// </summary>
    /// <value>the current stretched track position relative to <see cref="SyncedAt"/>.</value>
    public TimeSpan RelativePosition => TimeSpan.FromTicks((long)(UnstretchedRelativePosition.Ticks * TimeStretchFactor));

    public TimeSpan UnstretchedUnsyncedDuration => DateTimeOffset.UtcNow - SyncedAt;

    public TimeSpan UnsyncedDuration => TimeSpan.FromTicks((long)(UnstretchedUnsyncedDuration.Ticks * TimeStretchFactor));

    /// <summary>
    ///     Gets the current unstretched track position.
    /// </summary>
    /// <value>the current unstretched track position.</value>
    public TimeSpan UnstretchedPosition => UnstretchedRelativePosition + UnstretchedUnsyncedDuration;

    /// <summary>
    ///     Gets the current stretched track position.
    /// </summary>
    /// <value>the current stretched track position.</value>
    public TimeSpan Position => TimeSpan.FromTicks((long)((UnstretchedUnsyncedDuration.Ticks + UnstretchedRelativePosition.Ticks) * TimeStretchFactor));

    public float TimeStretchFactor { get; }

    public bool IsStretched => TimeStretchFactor is not 1F;
}
