namespace Lavalink4NET.Events
{
    using System;
    using Lavalink4NET.Player;

    public sealed class TrackStartedEventArgs : TrackEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TrackEventArgs"/> class.
        /// </summary>
        /// <param name="player">the affected player</param>
        /// <param name="trackIdentifier">the identifier of the affected track</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="player"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="trackIdentifier"/> is <see langword="null"/>.
        /// </exception>
        public TrackStartedEventArgs(LavalinkPlayer player, string trackIdentifier) : base(player, trackIdentifier)
        {
        }
    }
}