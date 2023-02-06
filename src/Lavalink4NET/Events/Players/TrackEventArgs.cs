namespace Lavalink4NET.Events.Players;

using System;
using Lavalink4NET.Players;
using Lavalink4NET.Tracks;

/// <summary>
///     The event arguments for the <see cref="LavalinkNode.TrackEnd"/>.
/// </summary>
public abstract class TrackEventArgs : EventArgs
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
    protected TrackEventArgs(ILavalinkPlayer player, LavalinkTrack track)
    {
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(track);

        Player = player;
        Track = track;
    }

    /// <summary>
    ///     Gets the affected player.
    /// </summary>
    public ILavalinkPlayer Player { get; }

    /// <summary>
    ///     Gets the affected track.
    /// </summary>
    public LavalinkTrack Track { get; }
}
