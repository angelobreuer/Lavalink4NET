namespace Lavalink4NET.Tracking
{
    using System;

    /// <summary>
    ///     A set of different inactivity tracking modes.
    /// </summary>
    [Flags]
    public enum InactivityTrackingMode
    {
        /// <summary>
        ///     Disables tracking.
        /// </summary>
        None = 0,

        /// <summary>
        ///     When this mode is specified the <see cref="InactivityTrackingService"/> will stop and
        ///     disconnect the player if no users are in the channel.
        /// </summary>
        User = 1,

        /// <summary>
        ///     When this mode is specified the <see cref="InactivityTrackingService"/> will stop and
        ///     disconnect the player if no track is playing.
        /// </summary>
        Track = 2,

        /// <summary>
        ///     The default tracking mode.
        /// </summary>
        Default = User | Track
    }
}