namespace Lavalink4NET.Players;

using Lavalink4NET.Tracks;

/// <summary>
///     Represents an item in the track queue.
/// </summary>
public interface ITrackQueueItem
{
    /// <summary>
    ///     Gets the enqueued track.
    /// </summary>
    /// <value>the enqueued track.</value>
    TrackReference Reference { get; }

    LavalinkTrack? Track => Reference.Track;

    string Identifier => Reference.Identifier!;

    T? As<T>() where T : class, ITrackQueueItem => this as T;
}