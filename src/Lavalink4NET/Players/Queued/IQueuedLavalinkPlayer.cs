namespace Lavalink4NET.Players.Queued;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Tracks;

public interface IQueuedLavalinkPlayer : ILavalinkPlayer
{
    ITrackQueueItem? CurrentItem { get; }

    ITrackQueue Queue { get; }

    TrackRepeatMode RepeatMode { get; set; }

    bool Shuffle { get; set; }

    ValueTask<int> PlayAsync(ITrackQueueItem queueItem, bool enqueue = true, TrackPlayProperties properties = default, CancellationToken cancellationToken = default);

    ValueTask<int> PlayAsync(LavalinkTrack track, bool enqueue = true, TrackPlayProperties properties = default, CancellationToken cancellationToken = default);

    ValueTask<int> PlayAsync(string identifier, bool enqueue = true, TrackPlayProperties properties = default, CancellationToken cancellationToken = default);

    ValueTask<int> PlayAsync(TrackReference trackReference, bool enqueue = true, TrackPlayProperties properties = default, CancellationToken cancellationToken = default);

    ValueTask SkipAsync(int count = 1, CancellationToken cancellationToken = default);
}