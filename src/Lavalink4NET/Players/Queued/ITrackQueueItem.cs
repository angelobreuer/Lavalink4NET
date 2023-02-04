namespace Lavalink4NET.Players.Queued;

/// <summary>
///     Represents an item in the track queue.
/// </summary>
public interface ITrackQueueItem
{
    /// <summary>
    ///     Gets the enqueued track.
    /// </summary>
    /// <value>the enqueued track.</value>
    TrackReference Track { get; }
}