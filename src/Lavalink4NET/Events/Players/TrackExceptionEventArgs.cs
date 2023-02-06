namespace Lavalink4NET.Events.Players;

using System;
using Lavalink4NET.Players;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;

/// <summary>
///     The event arguments for the <see cref="LavalinkNode.TrackException"/> event.
/// </summary>
public class TrackExceptionEventArgs : TrackEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TrackExceptionEventArgs"/> class.
    /// </summary>
    /// <param name="player">the affected player</param>
    /// <param name="trackIdentifier">the identifier of the affected track</param>
    /// <param name="errorMessage">an error message indicating what occurred.</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="player"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="trackIdentifier"/> is <see langword="null"/>.
    /// </exception>
    public TrackExceptionEventArgs(ILavalinkPlayer player, LavalinkTrack track, TrackException exception)
        : base(player, track)
    {
        Exception = exception;
    }

    public TrackException Exception { get; }
}
