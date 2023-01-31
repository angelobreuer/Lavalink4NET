namespace Lavalink4NET.Tracking;

using System;
using Lavalink4NET.Player;

/// <summary>
///     The event arguments for the
///     <see cref="InactivityTrackingService.PlayerTrackingStatusUpdated"/> event.
/// </summary>
public sealed class PlayerTrackingStatusUpdateEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PlayerTrackingStatusUpdateEventArgs"/> class.
    /// </summary>
    /// <param name="audioService">the audio service</param>
    /// <param name="player">the affected player (may be <see langword="null"/>)</param>
    /// <param name="trackingStatus">the new tracking status of the player</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="audioService"/> is <see langword="null"/>.
    /// </exception>
    public PlayerTrackingStatusUpdateEventArgs(IAudioService audioService,
        LavalinkPlayer player, InactivityTrackingStatus trackingStatus)
    {
        AudioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
        Player = player;
        TrackingStatus = trackingStatus;
    }

    /// <summary>
    ///     Gets the audio service.
    /// </summary>
    public IAudioService AudioService { get; }

    /// <summary>
    ///     Gets the affected player (may be <see langword="null"/>).
    /// </summary>
    public LavalinkPlayer Player { get; }

    /// <summary>
    ///     Gets the new tracking status of the player.
    /// </summary>
    public InactivityTrackingStatus TrackingStatus { get; }
}
