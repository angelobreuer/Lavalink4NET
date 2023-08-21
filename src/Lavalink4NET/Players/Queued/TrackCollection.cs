namespace Lavalink4NET.Players.Queued;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public abstract class TrackCollection : ITrackCollection
{
    protected TrackCollection()
    {
        Items = ImmutableList<ITrackQueueItem>.Empty;
        SyncRoot = new object();
    }

    public virtual ITrackQueueItem this[int index] => Items[index];

    public virtual int Count => Items.Count;

    public virtual bool IsEmpty => Count is 0;

    protected object SyncRoot { get; }

    protected ImmutableList<ITrackQueueItem> Items { get; set; }

    ValueTask<int> ITrackCollection.AddAsync(ITrackQueueItem item, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var count = Add(item);
        return new ValueTask<int>(count);
    }

    public virtual int Add(ITrackQueueItem item)
    {
        int count;
        lock (SyncRoot)
        {
            Items = Items.Add(item);
            count = Items.Count;
        }

        return count;
    }

    ValueTask<int> ITrackCollection.AddRangeAsync(IReadOnlyList<ITrackQueueItem> items, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var count = AddRange(items);
        return new ValueTask<int>(count);
    }

    public virtual int AddRange(IReadOnlyList<ITrackQueueItem> items)
    {
        int count;
        lock (SyncRoot)
        {
            Items = Items.AddRange(items);
            count = Items.Count;
        }

        return count;
    }

    ValueTask<int> ITrackCollection.ClearAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var count = Clear();
        return new ValueTask<int>(count);
    }

    public virtual int Clear()
    {
        int count;
        lock (SyncRoot)
        {
            count = Items.Count;
            Items = ImmutableList<ITrackQueueItem>.Empty;
        }

        return count;
    }

    public virtual bool Contains(ITrackQueueItem item)
    {
        return Items.Contains(item);
    }

    ValueTask<int> ITrackCollection.DistinctAsync(IEqualityComparer<ITrackQueueItem>? equalityComparer, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return new ValueTask<int>(Distinct(equalityComparer));
    }

    public virtual int Distinct(IEqualityComparer<ITrackQueueItem>? equalityComparer)
    {
        int difference;
        lock (SyncRoot)
        {
            var previousCount = Items.Count;
            Items = Items.ToHashSet(equalityComparer ?? TrackEqualityComparer.Instance).ToImmutableList();
            difference = previousCount - Items.Count;
        }

        return difference;
    }

    public virtual IEnumerator<ITrackQueueItem> GetEnumerator()
    {
        return ((IEnumerable<ITrackQueueItem>)Items).GetEnumerator();
    }

    ValueTask<int> ITrackCollection.RemoveAllAsync(Predicate<ITrackQueueItem> predicate, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var count = RemoveAll(predicate);

        return new ValueTask<int>(count);
    }

    public virtual int RemoveAll(Predicate<ITrackQueueItem> predicate)
    {
        int count;
        lock (SyncRoot)
        {
            var previousCount = Items.Count;
            Items = Items.RemoveAll(predicate);
            count = previousCount - Items.Count;
        }

        return count;
    }

    ValueTask<bool> ITrackCollection.RemoveAtAsync(int index, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var removed = RemoveAt(index);
        return new ValueTask<bool>(removed);
    }

    public virtual bool RemoveAt(int index)
    {
        bool removed;
        lock (SyncRoot)
        {
            if (index < 0 || index >= Items.Count)
            {
                removed = false;
            }
            else
            {
                Items = Items.RemoveAt(index);
                removed = true;
            }
        }

        return removed;
    }

    ValueTask<bool> ITrackCollection.RemoveAsync(ITrackQueueItem item, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var removed = Remove(item);
        return new ValueTask<bool>(removed);
    }

    public virtual bool Remove(ITrackQueueItem item)
    {
        bool removed;
        lock (SyncRoot)
        {
            var previousItems = Items;
            Items = Items.Remove(item);
            removed = !ReferenceEquals(previousItems, Items);
        }

        return removed;
    }

    ValueTask ITrackCollection.RemoveRangeAsync(int index, int count, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        RemoveRange(index, count);
        return default;
    }

    public virtual void RemoveRange(int index, int count)
    {
        lock (SyncRoot)
        {
            Items = Items.RemoveRange(index, count);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

file sealed class TrackEqualityComparer : IEqualityComparer<ITrackQueueItem>
{
    public static TrackEqualityComparer Instance { get; } = new TrackEqualityComparer();

    private static string? GetKey(ITrackQueueItem? item)
    {
        return item?.Reference.Identifier ?? item?.Reference.Identifier;
    }

    public bool Equals(ITrackQueueItem? x, ITrackQueueItem? y)
    {
        return GetKey(x) == GetKey(y);
    }

    public int GetHashCode([DisallowNull] ITrackQueueItem obj)
    {
        return GetKey(obj)?.GetHashCode() ?? 0;
    }
}