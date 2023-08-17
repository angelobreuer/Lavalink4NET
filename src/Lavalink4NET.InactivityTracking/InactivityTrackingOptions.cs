namespace Lavalink4NET.Tracking;

using System;
using System.Collections.Immutable;
using Lavalink4NET.InactivityTracking;

/// <summary>
///     The options for the <see cref="InactivityTrackingService"/>.
/// </summary>
public sealed class InactivityTrackingOptions
{
    /// <summary>
    ///     Gets or sets the default inactivity timeout for a player stop. Use <see cref="TimeSpan.Zero"/> for
    ///     disconnect immediately from the channel. The default timeout is used if a tracker does not specify a timeout.
    /// </summary>
    /// <remarks>This property defaults to <c>TimeSpan.FromSeconds(30)</c></remarks>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    ///     Gets or sets the poll interval for the <see cref="InactivityTrackingService"/> in
    ///     which the players should be tested for inactivity. This also affects the <see cref="DefaultTimeout"/>.
    /// </summary>
    /// <remarks>This property defaults to <c>TimeSpan.FromSeconds(5)</c></remarks>
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    ///     Gets or sets a value indicating whether the <see cref="InactivityTrackingService"/>
    ///     should start tracking inactive players after constructing it.
    /// </summary>
    /// <remarks>This property defaults to <see langword="false"/>.</remarks>
    public bool TrackInactivity { get; set; } = false;

    public ImmutableArray<IInactivityTracker>? Trackers { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to add the default trackers if no trackers were explicitly registered.
    /// </summary>
    public bool UseDefaultTrackers { get; set; } = true;
}
