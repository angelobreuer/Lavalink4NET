namespace Lavalink4NET.Tracking
{
    using System;
    using Lavalink4NET.Player;

    /// <summary>
    ///     The event arguments for the <see cref="InactivityTrackingService.InactivePlayer"/>.
    /// </summary>
    public sealed class InactivePlayerEventArgs : EventArgs
    {
        /// <summary>
        ///     Initialize a new instance of the <see cref="InactivePlayerEventArgs"/> class.
        /// </summary>
        /// <param name="audioService">the audio service</param>
        /// <param name="player">the affected player</param>
        public InactivePlayerEventArgs(IAudioService audioService, LavalinkPlayer player)
        {
            AudioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
            Player = player ?? throw new ArgumentNullException(nameof(player));
        }

        /// <summary>
        ///     Gets the audio service.
        /// </summary>
        public IAudioService AudioService { get; }

        /// <summary>
        ///     Gets the affected player.
        /// </summary>
        public LavalinkPlayer Player { get; }

        /// <summary>
        ///     Gets a value indicating whether the player should be stopped.
        /// </summary>
        public bool ShouldStop { get; set; } = true;
    }
}