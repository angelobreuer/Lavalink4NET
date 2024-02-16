namespace Lavalink4NET.Players.Queued;

using System.Diagnostics;
using Lavalink4NET.Tracks;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
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

    public override string ToString() => Reference.ToString();

    private string GetDebuggerDisplay() => Reference.GetDebuggerDisplay();
}