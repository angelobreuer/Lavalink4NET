namespace Lavalink4NET.Players.Queued;

using Lavalink4NET.Tracks;

public record class TrackQueueItem(TrackReference Reference) : ITrackQueueItem
{
    public TrackQueueItem(LavalinkTrack track)
        : this(new TrackReference(track))
    {
    }

    public TrackQueueItem(string identifier)
    : this(new TrackReference(identifier))
    {
    }
}