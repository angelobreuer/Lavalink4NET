namespace Lavalink4NET.Events;

using System;
using Player;

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
    protected TrackEventArgs(LavalinkPlayer player, string trackIdentifier)
    {
        Player = player ?? throw new ArgumentNullException(nameof(player));
        TrackIdentifier = trackIdentifier ?? throw new ArgumentNullException(nameof(trackIdentifier));
    }

    /// <summary>
    ///     Gets the identifier of the affected track.
    /// </summary>
    public string TrackIdentifier { get; }

    /// <summary>
    ///     Gets the affected player.
    /// </summary>
    public LavalinkPlayer Player { get; }
}
