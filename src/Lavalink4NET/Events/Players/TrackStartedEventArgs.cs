namespace Lavalink4NET.Events.Players;

using System;
using Lavalink4NET.Players;

/// <summary>
///     The event arguments ( <see cref="EventArgs"/>) for the <see
///     cref="IAudioService.TrackStarted"/> event.
/// </summary>
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
    public TrackStartedEventArgs(ILavalinkPlayer player, string trackIdentifier) : base(player, trackIdentifier)
    {
    }
}
