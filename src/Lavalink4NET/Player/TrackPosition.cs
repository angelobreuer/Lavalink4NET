/*
 *  File:   TrackPosition.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2022
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */

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
