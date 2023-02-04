namespace Lavalink4NET.Players.Queued;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>
///     A thread-safe implementation of a track queue with high-level features like shuffling or distinct.
/// </summary>
public class TrackQueue : IList<ITrackQueueItem>, IReadOnlyList<ITrackQueueItem>
{
    private readonly Stack<ITrackQueueItem>? _history;
    private readonly List<ITrackQueueItem> _queue;
    private readonly object _syncRoot;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TrackQueue"/> class with copying the
    ///     queue and history contents from the specified <paramref name="queue"/>.
    /// </summary>
    /// <param name="queue">the queue to copy the history and queue contents from.</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="queue"/> is <see langword="null"/>.
    /// </exception>
    public TrackQueue(TrackQueue queue)
    {
        ArgumentNullException.ThrowIfNull(queue);

        // clone everything except the synchronization root
        _syncRoot = new object();

        // lock on the queue synchronization root being cloned
        lock (queue._syncRoot)
        {
            HistoryCapacity = queue.HistoryCapacity;
            _queue = new List<ITrackQueueItem>(queue._queue);

            if (HistoryCapacity > 0)
            {
                _history = new Stack<ITrackQueueItem>(queue._history!);
            }
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TrackQueue"/> class.
    /// </summary>
    /// <param name="initialCapacity">the initial capacity.</param>
    /// <param name="historyCapacity">
    ///     the capacity of the history; or <c>0</c> to disable the queue history.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     thrown if the specified initial capacity ( <paramref name="initialCapacity"/>) is negative.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     thrown if the specified history capacity ( <paramref name="historyCapacity"/>) is negative.
    /// </exception>
    public TrackQueue(int initialCapacity = 5, int historyCapacity = 8)
    {
        if (initialCapacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialCapacity), initialCapacity,
                "The specified initial capacity must be greater than or equal to zero.");
        }

        if (historyCapacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(historyCapacity), historyCapacity,
                "The specified history capacity must be greater than or equal to zero.");
        }

        HistoryCapacity = historyCapacity;
        _syncRoot = new object();
        _queue = new List<ITrackQueueItem>(initialCapacity);

        if (historyCapacity > 0)
        {
            _history = new Stack<ITrackQueueItem>(historyCapacity);
        }
    }

    /// <inheritdoc/>
    public int Count
    {
        get
        {
            lock (_syncRoot)
            {
                return _queue.Count;
            }
        }
    }

    /// <summary>
    ///     Gets a value indicating whether the track queue remembers past tracks.
    /// </summary>
    /// <value>a value indicating whether the track queue remembers past tracks.</value>
    [MemberNotNullWhen(true, nameof(_history))]
    public bool HasHistory => _history != null;

    /// <summary>
    ///     Gets a read-only list containing the tracks dequeued in the past.
    /// </summary>
    /// <remarks>If the queue does not remember any past tracks an empty array is returned.</remarks>
    /// <value>a read-only list containing the tracks dequeued in the past.</value>
    public ImmutableArray<ITrackQueueItem> History
    {
        get
        {
            if (!HasHistory)
            {
                return ImmutableArray<ITrackQueueItem>.Empty;
            }

            lock (_syncRoot)
            {
                return _history.ToImmutableArray();
            }
        }
    }

    /// <summary>
    ///     Gets the capacity if the history.
    /// </summary>
    /// <value>the capacity if the history.</value>
    public int HistoryCapacity { get; }

    /// <summary>
    ///     Gets the number of elements stored in the track history.
    /// </summary>
    /// <remarks>If the queue does not remember any past tracks zero ( <c>0</c>) is returned.</remarks>
    /// <value>the number of elements stored in the track history.</value>
    public int HistorySize
    {
        get
        {
            if (!HasHistory)
            {
                return 0;
            }

            lock (_syncRoot)
            {
                return _history.Count;
            }
        }
    }

    /// <summary>
    ///     Gets a value indicating whether the track queue is empty.
    /// </summary>
    /// <value>a value indicating whether the track queue is empty.</value>
    public bool IsEmpty
    {
        get
        {
            lock (_syncRoot)
            {
                return _queue.Count is 0;
            }
        }
    }

    /// <inheritdoc/>
    bool ICollection<ITrackQueueItem>.IsReadOnly => false;

    /// <inheritdoc/>
    public ITrackQueueItem this[int index]
    {
        get
        {
            lock (_syncRoot)
            {
                return _queue[index];
            }
        }

        set
        {
            ArgumentNullException.ThrowIfNull(value);

            lock (_syncRoot)
            {
                _queue[index] = value;
            }
        }
    }

    /// <inheritdoc/>
    void ICollection<ITrackQueueItem>.Add(ITrackQueueItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (_syncRoot)
        {
            _queue.Add(item);
        }
    }

    /// <summary>
    ///     Clears the track queue.
    /// </summary>
    /// <returns>the number of tracks cleared.</returns>
    public int Clear()
    {
        lock (_syncRoot)
        {
            var items = _queue.Count;
            _queue.Clear();
            return items;
        }
    }

    /// <inheritdoc/>
    void ICollection<ITrackQueueItem>.Clear()
    {
        lock (_syncRoot)
        {
            _queue.Clear();
        }
    }

    /// <summary>
    ///     Clears the track history.
    /// </summary>
    /// <returns>the number of tracks cleared from the history.</returns>
    public int ClearHistory()
    {
        if (!HasHistory)
        {
            // queue has no history
            return 0;
        }

        lock (_syncRoot)
        {
            var items = _history.Count;
            _history.Clear();
            return items;
        }
    }

    /// <inheritdoc/>
    public TrackQueue Clone() => new(this);

    /// <inheritdoc/>
    public bool Contains(ITrackQueueItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (_syncRoot)
        {
            return _queue.Contains(item);
        }
    }

    /// <summary>
    ///     Copies the elements in the track queue to the specified <paramref name="array"/>.
    /// </summary>
    /// <param name="array">the array to copy to.</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="array"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     thrown if the specified <paramref name="array"/> has not enough space to the store
    ///     the queue items.
    /// </exception>
    public void CopyTo(ITrackQueueItem[] array) => CopyTo(array, arrayIndex: 0);

    /// <inheritdoc/>
    public void CopyTo(ITrackQueueItem[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);

        lock (_syncRoot)
        {
            _queue.CopyTo(array, arrayIndex);
        }
    }

    /// <summary>
    ///     Dequeues a track item from the queue.
    /// </summary>
    /// <returns>the track item dequeued from the queue.</returns>
    public ITrackQueueItem Dequeue() => Dequeue(shuffle: false);

    /// <summary>
    ///     Dequeues a track item from the queue.
    /// </summary>
    /// <param name="shuffle">
    ///     a value indicating whether to shuffle tracks, if <see langword="true"/> a a random
    ///     track is dequeued from the queue, if <see langword="false"/> the first track in the
    ///     queue is dequeued.
    /// </param>
    /// <returns>the track item dequeued from the queue.</returns>
    public ITrackQueueItem Dequeue(bool shuffle)
    {
        lock (_syncRoot)
        {
            if (_queue.Count == 0)
            {
                // no items in queue
                throw new InvalidOperationException("Queue is empty");
            }

            var index = shuffle ? Random.Shared.Next(_queue.Count) : 0;
            var item = _queue[index];
            _queue.RemoveAt(index);

            if (HasHistory)
            {
                if (_history.Count >= HistoryCapacity)
                {
                    // remove first element from the history to make place
                    _history.Pop();
                }

                // push the track into the history
                _history.Push(item);
            }

            return item;
        }
    }

    /// <summary>
    ///     Purges all duplicate elements in the queue.
    /// </summary>
    /// <returns>the number of elements removed.</returns>
    public int Distinct()
    {
        lock (_syncRoot)
        {
            if (_queue.Count <= 1)
            {
                // distinct would not make any changes
                return 0;
            }

            var previousCount = _queue.Count;

            var items = _queue
                .DistinctBy(x => x.Track.ToString())
                .ToArray();

            // add the items back to the queue
            _queue.Clear();
            _queue.AddRange(items);

            return previousCount - items.Length;
        }
    }

    /// <inheritdoc/>
    public int Enqueue(ITrackQueueItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (_syncRoot)
        {
            var index = _queue.Count;
            _queue.Add(item);
            return index;
        }
    }

    /// <summary>
    ///     Adds the items of the specified array ( <paramref name="items"/>) to the track queue.
    /// </summary>
    /// <param name="items">an enumerable that yields through the items to add.</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified array ( <paramref name="items"/>) is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     thrown if an item in the specified array ( <paramref name="items"/>) is <see langword="null"/>.
    /// </exception>
    public void EnqueueRange(params ITrackQueueItem[] items)
    {
        ArgumentNullException.ThrowIfNull(items);

        EnqueueRange((IEnumerable<ITrackQueueItem>)items);
    }

    /// <summary>
    ///     Adds the enumerable of the specified array ( <paramref name="items"/>) to the track queue.
    /// </summary>
    /// <param name="items">an enumerable that yields through the items to add.</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified array ( <paramref name="items"/>) is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     thrown if an item in the specified enumerable ( <paramref name="items"/>) is <see langword="null"/>.
    /// </exception>
    public void EnqueueRange(IEnumerable<ITrackQueueItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (items.Any(x => x is null))
        {
            throw new InvalidOperationException("An item was null.");
        }

        lock (_syncRoot)
        {
            _queue.AddRange(items);
        }
    }

    /// <inheritdoc/>
    public IEnumerator<ITrackQueueItem> GetEnumerator()
    {
        lock (_syncRoot)
        {
            return _queue.ToList().GetEnumerator();
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public int IndexOf(ITrackQueueItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (_syncRoot)
        {
            return _queue.IndexOf(item);
        }
    }

    /// <inheritdoc/>
    public void Insert(int index, ITrackQueueItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (_syncRoot)
        {
            _queue.Insert(index, item);
        }
    }

    /// <summary>
    ///     Inserts the items of the specified array ( <paramref name="items"/>) into the track
    ///     queue at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">the zero-based index at which the new items should be inserted.</param>
    /// <param name="items">an enumerable that yields through the items to add.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     thrown if the specified <paramref name="index"/> is less than <c>0</c> (zero).
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     thrown if the specified <paramref name="index"/> is greater than <see cref="Count"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified array ( <paramref name="items"/>) is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     thrown if an item in the specified array ( <paramref name="items"/>) is <see langword="null"/>.
    /// </exception>
    public void InsertRange(int index, params ITrackQueueItem[] items)
    {
        ArgumentNullException.ThrowIfNull(items);

        InsertRange(index, (IEnumerable<ITrackQueueItem>)items);
    }

    /// <summary>
    ///     Inserts the items of the specified enumerable ( <paramref name="items"/>) into the
    ///     track queue at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">the zero-based index at which the new items should be inserted.</param>
    /// <param name="items">an enumerable that yields through the items to add.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     thrown if the specified <paramref name="index"/> is less than <c>0</c> (zero).
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     thrown if the specified <paramref name="index"/> is greater than <see cref="Count"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified enumerable ( <paramref name="items"/>) is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     thrown if an item in the specified enumerable ( <paramref name="items"/>) is <see langword="null"/>.
    /// </exception>
    public void InsertRange(int index, IEnumerable<ITrackQueueItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (items.Any(x => x is null))
        {
            throw new InvalidOperationException("An item was null.");
        }

        lock (_syncRoot)
        {
            _queue.InsertRange(index, items);
        }
    }

    public ITrackQueueItem? Peek()
    {
        lock (_syncRoot)
        {
            return _queue.Count is 0 ? null : _queue[0];
        }
    }

    /// <inheritdoc/>
    public bool Remove(ITrackQueueItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (_syncRoot)
        {
            return _queue.Remove(item);
        }
    }

    /// <summary>
    ///     Removes all elements in the queue that match the specified <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">the predicate.</param>
    /// <returns>the number of total items removed from the queue.</returns>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="predicate"/> is <see langword="null"/>.
    /// </exception>
    public int RemoveAll(Predicate<ITrackQueueItem> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        lock (_syncRoot)
        {
            return _queue.RemoveAll(predicate);
        }
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        lock (_syncRoot)
        {
            _queue.RemoveAt(index);
        }
    }

    /// <summary>
    ///     Removes a range of items from the track queue.
    /// </summary>
    /// <param name="index">the zero-based starting index of the range of items to remove.</param>
    /// <param name="count">the number of items to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     thrown if the specified <paramref name="index"/> is less than <c>0</c> (zero).
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     thrown if the specified <paramref name="count"/> is less than <c>0</c> (zero).
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     thrown if the specified <paramref name="index"/> and the specified <paramref
    ///     name="count"/> do not denote a valid range of items in the queue.
    /// </exception>
    public void RemoveRange(int index, int count)
    {
        lock (_syncRoot)
        {
            _queue.RemoveRange(index, count);
        }
    }

    /// <summary>
    ///     Shuffles the whole queue.
    /// </summary>
    public void Shuffle() => Shuffle(index: 0, count: Count);

    /// <summary>
    ///     Shuffles the queue the specified range in the queue.
    /// </summary>
    /// <param name="index">the index to start shuffling items at.</param>
    /// <param name="count">the number of items to shuffle.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     thrown if the specified <paramref name="index"/> and <paramref name="count"/> is out
    ///     of bounds.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     thrown if the specified <paramref name="index"/> is less than <c>0</c> (zero).
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     thrown if the specified <paramref name="count"/> is less than <c>0</c> (zero).
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     thrown if the specified <paramref name="index"/> and the specified <paramref
    ///     name="count"/> do not denote a valid range of items in the queue.
    /// </exception>
    public void Shuffle(int index, int count)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "The specified index can not be less than 0.");
        }

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "The specified index can not be less than 0.");
        }

        lock (_syncRoot)
        {
            if (index + count > _queue.Count)
            {
                throw new ArgumentException("The specified index and count do not denote a valid range of items in the queue.");
            }

            for (; index < count - 1; index++)
            {
                var targetIndex = index + Random.Shared.Next(count - index);
                (_queue[index], _queue[targetIndex]) = (_queue[targetIndex], _queue[index]);
            }
        }
    }

    /// <summary>
    ///     Tries to dequeue a track from the queue.
    /// </summary>
    /// <param name="shuffle">
    ///     a value indicating whether to shuffle tracks, if <see langword="true"/> a a random
    ///     track is dequeued from the queue, if <see langword="false"/> the first track in the
    ///     queue is dequeued.
    /// </param>
    /// <param name="item">( <c>out</c>) the item dequeued.</param>
    /// <returns>a value indicating whether a track was dequeued.</returns>
    public bool TryDequeue(bool shuffle, [MaybeNullWhen(false)] out ITrackQueueItem item)
    {
        lock (_syncRoot)
        {
            if (_queue.Count == 0)
            {
                // no items in queue
                item = default;
                return false;
            }

            var index = shuffle ? Random.Shared.Next(_queue.Count) : 0;

            item = _queue[index];
            _queue.RemoveAt(index);

            if (HasHistory)
            {
                if (_history.Count >= HistoryCapacity)
                {
                    // remove first element from the history to make place
                    _history.Pop();
                }

                // push the track into the history
                _history.Push(item);
            }

            return true;
        }
    }

    /// <summary>
    ///     Tries to dequeue a track from the queue.
    /// </summary>
    /// <param name="item">( <c>out</c>) the item dequeued.</param>
    /// <returns>a value indicating whether a track was dequeued.</returns>
    public bool TryDequeue([MaybeNullWhen(false)] out ITrackQueueItem item)
        => TryDequeue(shuffle: false, out item);

    public bool TryPeek([MaybeNullWhen(false)] out ITrackQueueItem queueItem)
    {
        lock (_syncRoot)
        {
            if (_queue.Count is 0)
            {
                queueItem = default;
                return false;
            }

            queueItem = _queue[0];
        }

        return true;
    }
}