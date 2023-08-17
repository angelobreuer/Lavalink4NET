namespace Lavalink4NET.InactivityTracking.Events;

using System;
using Lavalink4NET.InactivityTracking;
using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;
using Lavalink4NET.Tracking;

/// <summary>
///     The event arguments for the
///     <see cref="InactivityTrackingService.TrackingStatusChanged"/> event.
/// </summary>
public sealed class TrackingStatusChangedEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TrackingStatusChangedEventArgs"/> class.
    /// </summary>
    /// <param name="player">the affected player (may be <see langword="null"/>)</param>
    /// <param name="trackingStatus">the new tracking status of the player</param>
    public TrackingStatusChangedEventArgs(
        IPlayerManager playerManager,
        ILavalinkPlayer player,
        PlayerTrackingState trackingState,
        PlayerTrackingState previousTrackingState)
    {
        ArgumentNullException.ThrowIfNull(playerManager);

        PlayerManager = playerManager;
        Player = player;
        TrackingState = trackingState;
        PreviousTrackingState = previousTrackingState;
    }

    public IPlayerManager PlayerManager { get; }

    /// <summary>
    ///     Gets the affected player.
    /// </summary>
    public ILavalinkPlayer Player { get; }

    public PlayerTrackingState TrackingState { get; }

    public PlayerTrackingState PreviousTrackingState { get; }

    /// <summary>
    ///     Gets the new tracking status of the player.
    /// </summary>
    public PlayerTrackingStatus TrackingStatus { get; }
}
