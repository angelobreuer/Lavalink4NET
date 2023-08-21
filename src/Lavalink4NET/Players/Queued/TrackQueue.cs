namespace Lavalink4NET.Players.Queued;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class TrackQueue : TrackCollection, ITrackQueue
{
    public TrackQueue(int? historyCapacity = 8)
    {
        History = historyCapacity > 0 ? new TrackHistory(historyCapacity) : null;
    }

    ValueTask ITrackQueue.InsertAsync(int index, ITrackQueueItem item, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Insert(index, item);
        return default;
    }

    public virtual void Insert(int index, ITrackQueueItem item)
    {
        lock (SyncRoot)
        {
            Items = Items.Insert(index, item);
        }
    }

    ValueTask ITrackQueue.InsertRangeAsync(int index, IEnumerable<ITrackQueueItem> items, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        InsertRange(index, items);
        return default;
    }

    public virtual void InsertRange(int index, IEnumerable<ITrackQueueItem> items)
    {
        lock (SyncRoot)
        {
            Items = Items.InsertRange(index, items);
        }
    }

    ValueTask ITrackQueue.ShuffleAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Shuffle();

        return default;
    }

    public virtual void Shuffle()
    {
        lock (SyncRoot)
        {
            var list = Items.ToBuilder();

            for (var index = 0; index < list.Count; index++)
            {
                var targetIndex = index + Random.Shared.Next(list.Count - index);
                (list[index], list[targetIndex]) = (list[targetIndex], list[index]);
            }

            Items = list.ToImmutable();
        }
    }

    public TrackHistory? History { get; }

    [MemberNotNullWhen(true, nameof(History))]
    public bool HasHistory => History is not null;

    ITrackHistory? ITrackQueue.History => History;

    public ITrackQueueItem? Peek()
    {
        lock (SyncRoot)
        {
            return Items.FirstOrDefault();
        }
    }

    ValueTask<ITrackQueueItem?> ITrackQueue.TryDequeueAsync(TrackDequeueMode dequeueMode, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return TryDequeue(dequeueMode, out var track) ? new ValueTask<ITrackQueueItem?>(track) : default;
    }

    private bool TryDequeue(TrackDequeueMode dequeueMode, out ITrackQueueItem? item)
    {
        lock (SyncRoot)
        {
            if (Items.IsEmpty)
            {
                item = null;
                return false;
            }

            var index = dequeueMode is TrackDequeueMode.Shuffle
                ? Random.Shared.Next(0, Items.Count)
                : 0;

            item = Items[index];
            Items = Items.RemoveAt(index);
        }

        History?.Add(item);

        return true;
    }

    public bool TryPeek([MaybeNullWhen(false)] out ITrackQueueItem? queueItem)
    {
        queueItem = Peek();
        return queueItem is not null;
    }
}
