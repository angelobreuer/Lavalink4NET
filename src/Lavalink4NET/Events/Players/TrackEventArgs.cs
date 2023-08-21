namespace Lavalink4NET.Events.Players;

using System;
using Lavalink4NET.Players;
using Lavalink4NET.Tracks;

public abstract class TrackEventArgs : PlayerEventArgs
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
		: base(player)
	{
		ArgumentNullException.ThrowIfNull(track);

		Track = track;
	}

	/// <summary>
	///     Gets the affected track.
	/// </summary>
	public LavalinkTrack Track { get; }
}
