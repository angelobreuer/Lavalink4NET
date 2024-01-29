namespace Lavalink4NET.Players.Queued;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players;

public interface ITrackCollection : IReadOnlyList<ITrackQueueItem>
{
    bool IsEmpty { get; }

    bool Contains(ITrackQueueItem item);

    int IndexOf(ITrackQueueItem item);

    int IndexOf(Func<ITrackQueueItem, bool> predicate);

    ValueTask<bool> RemoveAtAsync(int index, CancellationToken cancellationToken = default);

    ValueTask<bool> RemoveAsync(ITrackQueueItem item, CancellationToken cancellationToken = default);

    ValueTask<int> RemoveAllAsync(Predicate<ITrackQueueItem> predicate, CancellationToken cancellationToken = default);

    ValueTask RemoveRangeAsync(int index, int count, CancellationToken cancellationToken = default);

    ValueTask<int> DistinctAsync(IEqualityComparer<ITrackQueueItem>? equalityComparer = null, CancellationToken cancellationToken = default);

    ValueTask<int> AddAsync(ITrackQueueItem item, CancellationToken cancellationToken = default);

    ValueTask<int> AddRangeAsync(IReadOnlyList<ITrackQueueItem> items, CancellationToken cancellationToken = default);

    ValueTask<int> ClearAsync(CancellationToken cancellationToken = default);
}