namespace Lavalink4NET.Players.Queued;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players;

public interface ITrackQueue : ITrackCollection
{
    ValueTask InsertAsync(int index, ITrackQueueItem item, CancellationToken cancellationToken = default);

    ValueTask InsertRangeAsync(int index, IEnumerable<ITrackQueueItem> items, CancellationToken cancellationToken = default);

    ValueTask ShuffleAsync(CancellationToken cancellationToken = default);

    ITrackHistory? History { get; }

    [MemberNotNullWhen(true, nameof(History))]
    bool HasHistory { get; }

    ITrackQueueItem? Peek();

    bool TryPeek([MaybeNullWhen(false)] out ITrackQueueItem? queueItem);

    ValueTask<ITrackQueueItem?> TryDequeueAsync(
        TrackDequeueMode dequeueMode = TrackDequeueMode.Normal,
        CancellationToken cancellationToken = default);
}
