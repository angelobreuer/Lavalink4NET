namespace Lavalink4NET.Tracking
{
    using System;

    /// <summary>
    ///     The options for the <see cref="InactivityTrackingService"/>.
    /// </summary>
    public sealed class InactivityTrackingOptions
    {
        /// <summary>
        ///     Gets or sets a value indicating whether the first track (after using
        ///     <see cref="InactivityTrackingService.BeginTracking"/>) should be delayed using the <see cref="PollInterval"/>.
        /// </summary>
        /// <remarks>This property defaults to <see langword="true"/>.</remarks>
        public bool DelayFirstTrack { get; set; } = true;

        /// <summary>
        ///     Gets or sets the delay for a player stop. Use <see cref="TimeSpan.Zero"/> for
        ///     disconnect immediately from the channel.
        /// </summary>
        /// <remarks>This property defaults to <c>TimeSpan.FromSeconds(30)</c></remarks>
        public TimeSpan DisconnectDelay { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        ///     Gets or sets the poll interval for the <see cref="InactivityTrackingService"/> in
        ///     which the players should be tested for inactivity. This also affects the <see cref="DisconnectDelay"/>.
        /// </summary>
        /// <remarks>This property defaults to <c>TimeSpan.FromSeconds(5)</c></remarks>
        public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        ///     Gets or sets a value indicating whether the <see cref="InactivityTrackingService"/>
        ///     should start tracking inactive players after constructing it.
        /// </summary>
        /// <remarks>This property defaults to <see langword="false"/>.</remarks>
        public bool TrackInactivity { get; set; }
    }
}