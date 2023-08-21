namespace Lavalink4NET.Players;

using System;
using Microsoft.Extensions.Internal;

/// <summary>
///     Represents a track position.
/// </summary>
/// <param name="SyncedAt">the UTC time when the track position was last synced at.</param>
/// <param name="UnstretchedRelativePosition">the current unstretched track position relative to <see cref="SyncedAt"/>.</param>
/// <param name="TimeStretchFactor"></param>
public readonly record struct TrackPosition(ISystemClock SystemClock, DateTimeOffset SyncedAt, TimeSpan UnstretchedRelativePosition, float TimeStretchFactor = 1.0F)
{
    /// <summary>
    ///     Gets the current stretched track position relative to <see cref="SyncedAt"/>.
    /// </summary>
    /// <value>the current stretched track position relative to <see cref="SyncedAt"/>.</value>
    public TimeSpan RelativePosition => TimeSpan.FromTicks((long)(UnstretchedRelativePosition.Ticks * TimeStretchFactor));

    public TimeSpan UnstretchedUnsyncedDuration => SystemClock.UtcNow - SyncedAt;

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

    public bool IsStretched => TimeStretchFactor is not 1F;
}
