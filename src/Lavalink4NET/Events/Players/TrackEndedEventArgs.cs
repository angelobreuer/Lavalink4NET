namespace Lavalink4NET.Events.Players;

using System;
using Lavalink4NET.Players;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Tracks;

/// <summary>
///     The event arguments for the <see cref="LavalinkNode.TrackEnd"/> event.
/// </summary>
public class TrackEndedEventArgs : TrackEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TrackEndedEventArgs"/> class.
    /// </summary>
    /// <param name="player">the affected player</param>
    /// <param name="trackIdentifier">the identifier of the affected track</param>
    /// <param name="reason">the reason why the track ended</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="player"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="trackIdentifier"/> is <see langword="null"/>.
    /// </exception>
    public TrackEndedEventArgs(ILavalinkPlayer player, LavalinkTrack track, TrackEndReason reason)
        : base(player, track)
    {
        Reason = reason;
    }

    /// <summary>
    ///     Gets the reason why the track ended.
    /// </summary>
    public TrackEndReason Reason { get; }

    /// <summary>
    ///     Gets a value indicating whether the player should play the next track.
    /// </summary>
    public bool MayStartNext => Reason is TrackEndReason.Finished or TrackEndReason.LoadFailed;
}
