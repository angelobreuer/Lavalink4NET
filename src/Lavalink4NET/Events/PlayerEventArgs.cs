namespace Lavalink4NET.Events
{
    using System;
    using Lavalink4NET.Player;

    /// <summary>
    ///     Abstraction for event arguments where a player is affected.
    /// </summary>
    public abstract class PlayerEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerEventArgs"/> class.
        /// </summary>
        /// <param name="player">the affected player</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="player"/> is <see langword="null"/>.
        /// </exception>
        protected PlayerEventArgs(LavalinkPlayer player)
            => Player = player ?? throw new ArgumentNullException(nameof(player));

        /// <summary>
        ///     Gets the affected player.
        /// </summary>
        public LavalinkPlayer Player { get; }
    }
}