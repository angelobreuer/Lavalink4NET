namespace Lavalink4NET.InactivityTracking.Events;

using System;
using Lavalink4NET.Players;
using Lavalink4NET.Tracking;

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
    public PlayerTrackingStatusUpdateEventArgs(
        IAudioService audioService,
        ulong guildId,
        ILavalinkPlayer? player,
        InactivityTrackingStatus trackingStatus)
    {
        ArgumentNullException.ThrowIfNull(audioService);

        AudioService = audioService;
        GuildId = guildId;
        Player = player;
        TrackingStatus = trackingStatus;
    }

    /// <summary>
    ///     Gets the audio service.
    /// </summary>
    public IAudioService AudioService { get; }

    public ulong GuildId { get; }

    /// <summary>
    ///     Gets the affected player (may be <see langword="null"/>).
    /// </summary>
    public ILavalinkPlayer? Player { get; }

    /// <summary>
    ///     Gets the new tracking status of the player.
    /// </summary>
    public InactivityTrackingStatus TrackingStatus { get; }
}
