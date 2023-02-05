namespace Lavalink4NET.Tests.Players;

using System;
using System.Collections.Generic;
using System.Linq;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Xunit;

/// <summary>
///     This class contains unit tests for the <see cref="TrackQueue"/> class.
/// </summary>
public sealed class TrackQueueTests
{
    [Fact]
    public void CopyConstructorWithNullQueue()
    {
        static void Test() => _ = new TrackQueue(null!);
        Assert.Throws<ArgumentNullException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.CopyTo(ITrackQueueItem[])"/> method of the <see
    ///     cref="TrackQueue"/> class.
    /// </summary>
    [Fact]
    public void TestArrayCopyTo()
    {
        var queue = new TrackQueue();
        var item = GetDummyTrack();
        queue.Enqueue(item);

        var array = new ITrackQueueItem[1];
        queue.CopyTo(array);

        Assert.Equal(item, array[0]);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.CopyTo(ITrackQueueItem[])"/> method of the <see
    ///     cref="TrackQueue"/> class with passing a <see langword="null"/> array which should
    ///     throw an <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void TestArrayCopyToWithNullArrayShouldThrow()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.CopyTo(null!);
        };

        Assert.Throws<ArgumentNullException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.CopyTo(ITrackQueueItem[], int)"/> method of the <see
    ///     cref="TrackQueue"/> class with specifying a copy offset.
    /// </summary>
    [Fact]
    public void TestArrayCopyToWithOffset()
    {
        var queue = new TrackQueue();
        var item = GetDummyTrack();
        queue.Enqueue(item);

        var array = new ITrackQueueItem[2];
        queue.CopyTo(array, 1);

        Assert.Null(array[0]);
        Assert.Equal(item, array[1]);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.CopyTo(ITrackQueueItem[])"/> method of the <see
    ///     cref="TrackQueue"/> class with passing an array which can not hold the tracks in the
    ///     queue which should thrown an <see cref="ArgumentException"/>.
    /// </summary>
    [Fact]
    public void TestArrayCopyToWithTooSmallArrayShouldThrow()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.Enqueue(GetDummyTrack());
            queue.CopyTo(new ITrackQueueItem[0]);
        };

        Assert.Throws<ArgumentException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue"/> constructor of the <see cref="TrackQueue"/> class
    ///     with specifying a history.
    /// </summary>
    [Fact]
    public void TestCanCreateWithHistory()
    {
        var queue = new TrackQueue(historyCapacity: 10);
        Assert.True(queue.HasHistory);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue"/> constructor of the <see cref="TrackQueue"/> class
    ///     without specifying a history.
    /// </summary>
    [Fact]
    public void TestCanCreateWithoutHistory()
    {
        var queue = new TrackQueue(historyCapacity: 0);
        Assert.False(queue.HasHistory);
    }

    [Fact]
    public void TestClearHistoryWhenDisabled()
    {
        var queue = new TrackQueue(historyCapacity: 0);
        Assert.Equal(0, queue.ClearHistory());
    }

    [Fact]
    public void TestClone()
    {
        var queue = new TrackQueue();
        var copiedQueue = queue.Clone();

        Assert.NotSame(queue, copiedQueue);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Contains(ITrackQueueItem)"/> method of the <see
    ///     cref="TrackQueue"/> class.
    /// </summary>
    [Fact]
    public void TestContains()
    {
        var queue = new TrackQueue();
        var item = GetDummyTrack();
        Assert.DoesNotContain(item, queue);
        queue.Enqueue(item);
        Assert.Contains(item, queue);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Contains(ITrackQueueItem)"/> method of the <see
    ///     cref="TrackQueue"/> class with passing a <see langword="null"/> item which should
    ///     throw an <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void TestContainsWithNullItemShouldThrow()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.Enqueue(GetDummyTrack());
            queue.Contains(null!);
        };

        Assert.Throws<ArgumentNullException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue"/> constructor of the <see cref="TrackQueue"/> class
    ///     with passing an invalid history capacity.
    /// </summary>
    [Fact]
    public void TestCreateWithInvalidHistoryCapacityShouldThrow()
    {
        static void Test() => new TrackQueue(historyCapacity: -1);
        Assert.Throws<ArgumentOutOfRangeException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue"/> constructor of the <see cref="TrackQueue"/> class
    ///     with passing an invalid initial capacity.
    /// </summary>
    [Fact]
    public void TestCreateWithInvalidInitialCapacityShouldThrow()
    {
        static void Test() => _ = new TrackQueue(initialCapacity: -1);
        Assert.Throws<ArgumentOutOfRangeException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Dequeue()"/> method of the <see cref="TrackQueue"/>
    ///     class with items in the queue.
    /// </summary>
    [Fact]
    public void TestDequeueWithItems()
    {
        var queue = new TrackQueue();

        var track = GetDummyTrack();
        queue.Enqueue(track);
        Assert.Equal(track, queue.Dequeue());
    }

    [Fact]
    public void TestDequeueWithShuffleReturnsRandomElement()
    {
        var queue = new TrackQueue();
        var item = GetDummyTrack();

        queue.Enqueue(item);

        queue.EnqueueRange(
            Enumerable.Repeat<object?>(null, 50)
            .Select(x => GetDummyTrack()));

        for (var index = 0; index < 10; index++)
        {
            var result = queue.Dequeue(true);

            if (!result!.Equals(item))
            {
                return; // success
            }

            // retry
            queue.Insert(0, item);
        }

        Assert.True(false, "Test failed after multiple tries.");
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Distinct()"/> method of the <see cref="TrackQueue"/>
    ///     class with different items in the queue.
    /// </summary>
    [Fact]
    public void TestDistinctWithDifferentElements()
    {
        var queue = new TrackQueue();
        queue.Enqueue(GetDummyTrack());
        queue.Enqueue(GetDummyTrack());

        queue.Distinct();
        Assert.Equal(2, queue.Count);
    }

    [Fact]
    public void TestDistinctWithoutMultipleItems()
    {
        var queue = new TrackQueue(historyCapacity: 0);
        Assert.Equal(0, queue.Distinct());
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Distinct()"/> method of the <see cref="TrackQueue"/>
    ///     class with the same items in the queue.
    /// </summary>
    [Fact]
    public void TestDistinctWithSameElements()
    {
        var queue = new TrackQueue();
        var track = GetDummyTrack();

        queue.Enqueue(track);
        queue.Enqueue(track);

        queue.Distinct();
        Assert.Single(queue);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Enqueue(ITrackQueueItem)"/> method of the <see
    ///     cref="TrackQueue"/> class.
    /// </summary>
    [Fact]
    public void TestEnqueue()
    {
        var queue = new TrackQueue();
        Assert.Equal(0, queue.Enqueue(GetDummyTrack()));
        Assert.Equal(1, queue.Enqueue(GetDummyTrack()));
        Assert.Equal(2, queue.Enqueue(GetDummyTrack()));
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.EnqueueRange(ITrackQueueItem[])"/> method of the
    ///     <see cref="TrackQueue"/> class.
    /// </summary>
    [Fact]
    public void TestEnqueueRange()
    {
        var queue = new TrackQueue();
        queue.EnqueueRange(GetDummyTrack(), GetDummyTrack());
        Assert.Equal(2, queue.Count);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.EnqueueRange(ITrackQueueItem[])"/> method of the
    ///     <see cref="TrackQueue"/> class with passing a <see langword="null"/> array which
    ///     should throw an <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void TestEnqueueRangeWithNullArrayShouldThrow()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.EnqueueRange(items: null!);
        };

        Assert.Throws<ArgumentNullException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.EnqueueRange(IEnumerable{ITrackQueueItem})"/> method
    ///     of the <see cref="TrackQueue"/> class with passing a <see langword="null"/>
    ///     enumerable which should throw an <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void TestEnqueueRangeWithNullEnumerableShouldThrow()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.EnqueueRange(items: (IEnumerable<ITrackQueueItem>)null!);
        };

        Assert.Throws<ArgumentNullException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.EnqueueRange(ITrackQueueItem[])"/> method of the
    ///     <see cref="TrackQueue"/> class with passing an array that contains a <see
    ///     langword="null"/> reference for an item which should throw an <see cref="InvalidOperationException"/>.
    /// </summary>
    [Fact]
    public void TestEnqueueRangeWithNullItemInArrayShouldThrow()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.EnqueueRange(items: new ITrackQueueItem[] { GetDummyTrack(), null! });
        };

        Assert.Throws<InvalidOperationException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.EnqueueRange(IEnumerable{ITrackQueueItem})"/> method
    ///     of the <see cref="TrackQueue"/> class with passing an enumerable that contains a
    ///     <see langword="null"/> reference for an item which should throw an <see cref="InvalidOperationException"/>.
    /// </summary>
    [Fact]
    public void TestEnqueueRangeWithNullItemInEnumerableShouldThrow()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.EnqueueRange(items: (IEnumerable<ITrackQueueItem>)new ITrackQueueItem[] { GetDummyTrack(), null! });
        };

        Assert.Throws<InvalidOperationException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Enqueue(ITrackQueueItem)"/> method of the <see
    ///     cref="TrackQueue"/> class with passing a <see langword="null"/> item which should
    ///     throw a <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void TestEnqueueWithNullItemShouldThrow()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.Enqueue(null!);
        };

        Assert.Throws<ArgumentNullException>(Test);
    }

    [Fact]
    public void TestGetHistoryWhenDisabled()
    {
        var queue = new TrackQueue(historyCapacity: 0);
        Assert.False(queue.HasHistory);
        Assert.Empty(queue.History);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue"/><see cref="this"/> indexer of the <see
    ///     cref="TrackQueue"/> class with passing a negative index which should throw an <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Fact]
    public void TestGetQueueItemWithNegativeIndexShouldThrow()
    {
        static void Test() => _ = new TrackQueue()[-1];
        Assert.Throws<ArgumentOutOfRangeException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue"/><see cref="this"/> indexer of the <see
    ///     cref="TrackQueue"/> class with passing a non-existent index (out of range) which
    ///     should throw an <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Fact]
    public void TestGetQueueItemWithNotExistientIndexShouldThrow()
    {
        static void Test() => _ = new TrackQueue()[0];
        Assert.Throws<ArgumentOutOfRangeException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.ClearHistory()"/> method of the <see
    ///     cref="TrackQueue"/> class.
    /// </summary>
    [Fact]
    public void TestHistoryEmptyAfterClear()
    {
        var queue = new TrackQueue();
        Assert.Empty(queue);

        var track = GetDummyTrack();
        queue.Enqueue(track);
        Assert.Equal(track, queue.Dequeue());

        Assert.NotEmpty(queue.History);
        queue.ClearHistory();
        Assert.Empty(queue.History);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue"/> constructor of the <see cref="TrackQueue"/> class
    ///     with checking that the queue is initially empty.
    /// </summary>
    [Fact]
    public void TestHistoryInitialEmpty()
    {
        var queue = new TrackQueue();
        Assert.Empty(queue.History);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Dequeue()"/> method of the <see cref="TrackQueue"/>
    ///     class with checking that the queue's history is not empty after dequeuing an item.
    /// </summary>
    [Fact]
    public void TestHistoryNotEmptyAfterDequeue()
    {
        var queue = new TrackQueue();
        queue.Enqueue(GetDummyTrack());
        queue.Dequeue();
        Assert.Single(queue.History);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.HistorySize"/> property of the <see
    ///     cref="TrackQueue"/> class with checking that the history size increases when an item
    ///     is added to the history.
    /// </summary>
    [Fact]
    public void TestHistorySizeIncrementingWhenTrackIsDequeued()
    {
        var queue = new TrackQueue();
        Assert.Equal(0, queue.Enqueue(GetDummyTrack()));
        Assert.Equal(0, queue.HistorySize);

        queue.Dequeue();
        Assert.Equal(1, queue.HistorySize);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.HistorySize"/> property of the <see
    ///     cref="TrackQueue"/> class with checking that the history size is limited to the
    ///     specified size.
    /// </summary>
    [Fact]
    public void TestHistorySizeIsLimited()
    {
        var queue = new TrackQueue(historyCapacity: 4);

        for (var index = 0; index < 8; index++)
        {
            queue.Enqueue(GetDummyTrack());
            queue.Dequeue();
        }

        Assert.Equal(4, queue.HistorySize);
        Assert.Empty(queue);
    }

    [Fact]
    public void TestHistorySizeWhenDisabled()
    {
        var queue = new TrackQueue(historyCapacity: 0);
        Assert.Equal(0, queue.HistorySize);
    }

    [Fact]
    public void TestICollectionAdd()
    {
        var queue = new TrackQueue();
        var item = GetDummyTrack();
        ((ICollection<ITrackQueueItem>)queue).Add(item);
        Assert.Single(queue);
    }

    [Fact]
    public void TestICollectionAddWithNullItemThrows()
    {
        var queue = new TrackQueue();
        void Test() => ((ICollection<ITrackQueueItem>)queue).Add(null!);
        Assert.Throws<ArgumentNullException>(Test);
    }

    [Fact]
    public void TestICollectionClear()
    {
        var queue = new TrackQueue();
        var item = GetDummyTrack();
        queue.Enqueue(item);
        ((ICollection<ITrackQueueItem>)queue).Clear();
        Assert.Empty(queue);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Insert(int, ITrackQueueItem)"/> method of the <see
    ///     cref="TrackQueue"/> class.
    /// </summary>
    [Fact]
    public void TestInsertQueue()
    {
        var queue = new TrackQueue();
        queue.Insert(0, GetDummyTrack());
        Assert.NotEmpty(queue);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Insert(int, ITrackQueueItem)"/> method of the <see
    ///     cref="TrackQueue"/> class with passing a negative index which should throw a <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Fact]
    public void TestInsertQueueItemWithNegativeIndexShouldThrow()
    {
        static void Test() => new TrackQueue().Insert(-1, GetDummyTrack());
        Assert.Throws<ArgumentOutOfRangeException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Insert(int, ITrackQueueItem)"/> method of the <see
    ///     cref="TrackQueue"/> class with passing a <see langword="null"/> reference for the
    ///     item which should throw a <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void TestInsertQueueItemWithNullItemShouldThrow()
    {
        static void Test() => new TrackQueue().Insert(0, null!);
        Assert.Throws<ArgumentNullException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.InsertRange(int, ITrackQueueItem[])"/> method of the
    ///     <see cref="TrackQueue"/> class.
    /// </summary>
    [Fact]
    public void TestInsertRange()
    {
        var queue = new TrackQueue();
        queue.InsertRange(0, GetDummyTrack(), GetDummyTrack());
        Assert.Equal(2, queue.Count);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.InsertRange(int, ITrackQueueItem[])"/> method of the
    ///     <see cref="TrackQueue"/> class with specifying a <see langword="null"/> reference
    ///     for the array which should throw an <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void TestInsertRangeWithNullArrayShouldThrow()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.InsertRange(0, items: null!);
        }

        Assert.Throws<ArgumentNullException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.InsertRange(int, IEnumerable{ITrackQueueItem})"/>
    ///     method of the <see cref="TrackQueue"/> class with specifying a <see
    ///     langword="null"/> reference for the enumerable which should throw an <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void TestInsertRangeWithNullEnumerableShouldThrow()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.InsertRange(0, items: (IEnumerable<ITrackQueueItem>)null!);
        }

        Assert.Throws<ArgumentNullException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.InsertRange(int, ITrackQueueItem[])"/> method of the
    ///     <see cref="TrackQueue"/> class with specifying a <see langword="null"/> reference in
    ///     the array for an item which should throw an <see cref="InvalidOperationException"/>.
    /// </summary>
    [Fact]
    public void TestInsertRangeWithNullItemInArrayShouldThrow()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.InsertRange(0, items: new ITrackQueueItem[] { GetDummyTrack(), null! });
        }

        Assert.Throws<InvalidOperationException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.InsertRange(int, IEnumerable{ITrackQueueItem})"/>
    ///     method of the <see cref="TrackQueue"/> class with specifying a <see
    ///     langword="null"/> reference in the enumerable for an item which should throw an <see cref="InvalidOperationException"/>.
    /// </summary>
    [Fact]
    public void TestInsertRangeWithNullItemInEnumerableShouldThrow()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.InsertRange(0, items: (IEnumerable<ITrackQueueItem>)new ITrackQueueItem[] { GetDummyTrack(), null! });
        }

        Assert.Throws<InvalidOperationException>(Test);
    }

    [Fact]
    public void TestPeekWithItems()
    {
        var queue = new TrackQueue();

        var track = GetDummyTrack();
        queue.Enqueue(track);
        Assert.Equal(track, queue.Peek());
    }

    [Fact]
    public void TestPeekWithNoItems()
    {
        var queue = new TrackQueue();
        Assert.Null(queue.Peek());
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Count"/> property of the <see cref="TrackQueue"/> class.
    /// </summary>
    [Fact]
    public void TestQueueCountOnEmptyQueue()
    {
        var queue = new TrackQueue();
        Assert.Empty(queue);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Count"/> property of the <see cref="TrackQueue"/> class.
    /// </summary>
    [Fact]
    public void TestQueueCountOnQueueWithItems()
    {
        var queue = new TrackQueue();
        queue.Enqueue(GetDummyTrack());
        queue.Enqueue(GetDummyTrack());

        Assert.Equal(2, queue.Count);
        queue.Dequeue();
        Assert.Single(queue);
        queue.Dequeue();
        Assert.Empty(queue);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Dequeue()"/> method of the <see cref="TrackQueue"/>
    ///     class on an empty queue which should throw an <see cref="InvalidOperationException"/>.
    /// </summary>
    [Fact]
    public void TestQueueDequeueThrowsIfEmpty()
    {
        var queue = new TrackQueue();
        void Test() => queue.Dequeue();
        Assert.Throws<InvalidOperationException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.IsEmpty"/> property of the <see cref="TrackQueue"/>
    ///     class after adding and removing items.
    /// </summary>
    [Fact]
    public void TestQueueEmptyAfterClear()
    {
        var queue = new TrackQueue();
        Assert.Empty(queue);

        var track = GetDummyTrack();
        queue.Enqueue(track);

        Assert.NotEmpty(queue);
        queue.Clear();
        Assert.Empty(queue);
    }

    [Fact]
    public void TestQueueEmptyIfEmpty()
    {
        var queue = new TrackQueue();
        Assert.True(queue.IsEmpty);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.IsEmpty"/> property of the <see cref="TrackQueue"/>
    ///     class on a fresh initialized queue.
    /// </summary>
    [Fact]
    public void TestQueueEmptyOnEmptyQueue()
    {
        var queue = new TrackQueue();
        Assert.Empty(queue);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.IsEmpty"/> property of the <see cref="TrackQueue"/>
    ///     class on a queue with inserted items.
    /// </summary>
    [Fact]
    public void TestQueueEmptyOnQueueWithItems()
    {
        var queue = new TrackQueue();

        Assert.Empty(queue);
        queue.Enqueue(GetDummyTrack());
        Assert.Single(queue);
        queue.Dequeue();
        Assert.Empty(queue);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.IndexOf(ITrackQueueItem)"/> method of the <see
    ///     cref="TrackQueue"/> class.
    /// </summary>
    [Fact]
    public void TestQueueIndexOf()
    {
        var track1 = GetDummyTrack();
        var track2 = GetDummyTrack();
        var queue = new TrackQueue();

        queue.Enqueue(track1);
        queue.Enqueue(track2);

        Assert.Equal(0, queue.IndexOf(track1));
        Assert.Equal(1, queue.IndexOf(track2));
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.IndexOf(ITrackQueueItem)"/> method of the <see
    ///     cref="TrackQueue"/> class with passing an item that is not in the queue.
    /// </summary>
    [Fact]
    public void TestQueueIndexOfNotExisting()
    {
        var track1 = GetDummyTrack();
        var track2 = GetDummyTrack();
        var queue = new TrackQueue();

        queue.Enqueue(track1);
        Assert.Equal(-1, queue.IndexOf(track2));
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.IndexOf(ITrackQueueItem)"/> method of the <see
    ///     cref="TrackQueue"/> class with passing a <see langword="null"/> reference for the
    ///     item which should throw an <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void TestQueueIndexOfShouldThrowIfNull()
    {
        static void Test() => new TrackQueue().IndexOf(null!);
        Assert.Throws<ArgumentNullException>(Test);
    }

    [Fact]
    public void TestQueueIsReadOnlyAlwaysFalse()
    {
        var queue = new TrackQueue();
        Assert.False(((ICollection<ITrackQueueItem>)queue).IsReadOnly);
    }

    [Fact]
    public void TestRemoveAll()
    {
        var queue = new TrackQueue();
        var track = GetDummyTrack();

        queue.EnqueueRange(track, GetDummyTrack(), GetDummyTrack());

        Assert.Equal(1, queue.RemoveAll(x => x == track));
        Assert.Equal(2, queue.Count);
    }

    [Fact]
    public void TestRemoveAllWithNullPredicatePassedThrows()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.RemoveAll(null!);
        }

        Assert.Throws<ArgumentNullException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.RemoveAt(int)"/> method of the <see
    ///     cref="TrackQueue"/> class.
    /// </summary>
    [Fact]
    public void TestRemoveAt()
    {
        var queue = new TrackQueue();
        queue.Enqueue(GetDummyTrack());
        queue.RemoveAt(0);
        Assert.Empty(queue);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.RemoveAt(int)"/> method of the <see
    ///     cref="TrackQueue"/> class with passing a negative index which should throw an <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Fact]
    public void TestRemoveAtWithNegativeIndexShouldThrow()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.Enqueue(GetDummyTrack());
            queue.RemoveAt(-1);
        }

        Assert.Throws<ArgumentOutOfRangeException>(Test);
    }

    [Fact]
    public void TestRemoveRange()
    {
        var queue = new TrackQueue();

        queue.EnqueueRange(GetDummyTrack(), GetDummyTrack(), GetDummyTrack(), GetDummyTrack());
        queue.RemoveRange(2, 2);

        Assert.Equal(2, queue.Count);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Remove(ITrackQueueItem)"/> method of the <see
    ///     cref="TrackQueue"/> class with removing an existing item.
    /// </summary>
    [Fact]
    public void TestRemoveWithExistientItem()
    {
        var track = GetDummyTrack();
        var queue = new TrackQueue();
        queue.Enqueue(track);
        Assert.True(queue.Remove(track));
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Remove(ITrackQueueItem)"/> method of the <see
    ///     cref="TrackQueue"/> class with removing a non-existing item.
    /// </summary>
    [Fact]
    public void TestRemoveWithNonExistientItem()
    {
        var queue = new TrackQueue();
        Assert.False(queue.Remove(GetDummyTrack()));
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Remove(ITrackQueueItem)"/> method of the <see
    ///     cref="TrackQueue"/> class with specifying a <see langword="null"/> reference for the
    ///     item to remove which should throw an <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void TestRemoveWithNullItemShouldThrow()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.Remove(null!);
        }

        Assert.Throws<ArgumentNullException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue"/><see cref="this"/> indexer of the <see
    ///     cref="TrackQueue"/> class on an existing item.
    /// </summary>
    [Fact]
    public void TestSetQueueItemWithExistentIndex()
    {
        var item = GetDummyTrack();
        var queue = new TrackQueue();
        queue.Enqueue(item);
        queue[0] = item;
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue"/><see cref="this"/> indexer of the <see
    ///     cref="TrackQueue"/> class specifying a negative index which should throw an <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Fact]
    public void TestSetQueueItemWithNegativeIndexShouldThrow()
    {
        static void Test() => new TrackQueue()[-1] = GetDummyTrack();
        Assert.Throws<ArgumentOutOfRangeException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue"/><see cref="this"/> indexer of the <see
    ///     cref="TrackQueue"/> class specifying a non-existent index which should throw an <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Fact]
    public void TestSetQueueItemWithNotExistientIndex()
    {
        static void Test() => new TrackQueue()[0] = GetDummyTrack();
        Assert.Throws<ArgumentOutOfRangeException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue"/><see cref="this"/> indexer of the <see
    ///     cref="TrackQueue"/> class with trying to set a <see langword="null"/> reference for
    ///     an existent index which should throw an <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void TestSetQueueWithNullItemWithItemsShouldThrow()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.Enqueue(GetDummyTrack());
            queue[0] = null!;
        }

        Assert.Throws<ArgumentNullException>(Test);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.Shuffle"/> method of the <see cref="TrackQueue"/> class.
    /// </summary>
    [Fact]
    public void TestShuffle()
    {
        var queue = new TrackQueue();

        for (var index = 0; index < 100; index++)
        {
            queue.Enqueue(GetDummyTrack());
        }

        var shuffledQueue = queue.Clone();

        shuffledQueue.Shuffle();
        Assert.False(queue.SequenceEqual(shuffledQueue));
    }

    [Fact]
    public void TestShuffleThrowsIfIndexAndCountIsOutOfRangePassed()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.Shuffle(1, 1);
        }

        Assert.Throws<ArgumentException>(Test);
    }

    [Fact]
    public void TestShuffleThrowsIfNegativeCountPassed()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.Shuffle(0, -1);
        }

        Assert.Throws<ArgumentOutOfRangeException>(Test);
    }

    [Fact]
    public void TestShuffleThrowsIfNegativeIndexPassed()
    {
        static void Test()
        {
            var queue = new TrackQueue();
            queue.Shuffle(-1, 0);
        }

        Assert.Throws<ArgumentOutOfRangeException>(Test);
    }

    [Fact]
    public void TestTryDequeueWithHistoryCutOff()
    {
        var queue = new TrackQueue();

        queue.EnqueueRange(
            Enumerable.Repeat<object?>(null, 50)
            .Select(x => GetDummyTrack()));

        while (!queue.IsEmpty)
        {
            queue.TryDequeue(out _);
        }

        Assert.Equal(queue.HistoryCapacity, queue.HistorySize);
    }

    [Fact]
    public void TestTryDequeueWithItem()
    {
        var queue = new TrackQueue(historyCapacity: 0);
        var item = GetDummyTrack();

        queue.Enqueue(item);

        Assert.True(queue.TryDequeue(out var result));
        Assert.Equal(item, result);
    }

    /// <summary>
    ///     Tests the <see cref="TrackQueue.TryDequeue(out ITrackQueueItem)"/> method of the
    ///     <see cref="TrackQueue"/> class without any items.
    /// </summary>
    [Fact]
    public void TestTryDequeueWithoutItems()
    {
        var queue = new TrackQueue();
        Assert.False(queue.TryDequeue(out var _));
    }

    [Fact]
    public void TestTryDequeueWithShuffleReturnsRandomElement()
    {
        var queue = new TrackQueue();
        var item = GetDummyTrack();

        queue.Enqueue(item);

        queue.EnqueueRange(
            Enumerable.Repeat<object?>(null, 50)
            .Select(x => GetDummyTrack()));

        for (var index = 0; index < 10; index++)
        {
            queue.TryDequeue(true, out var result);

            if (!result!.Equals(item))
            {
                return; // success
            }

            // retry
            queue.Insert(0, item);
        }

        Assert.True(false, "Test failed after multiple tries.");
    }

    [Fact]
    public void TestTryDequeueWithShuffleReturnsRandomElementWithoutHistory()
    {
        var queue = new TrackQueue(historyCapacity: 0);
        var item = GetDummyTrack();

        queue.Enqueue(item);

        queue.EnqueueRange(
            Enumerable.Repeat<object?>(null, 50)
            .Select(x => GetDummyTrack()));

        for (var index = 0; index < 10; index++)
        {
            queue.TryDequeue(true, out var result);

            if (!result!.Equals(item))
            {
                return; // success
            }

            // retry
            queue.Insert(0, item);
        }

        Assert.True(false, "Test failed after multiple tries.");
    }

    [Fact]
    public void TestTryPeekWithItems()
    {
        var queue = new TrackQueue();

        var track = GetDummyTrack();
        queue.Enqueue(track);

        Assert.True(queue.TryPeek(out var peekedTrack));
        Assert.Equal(track, peekedTrack);
    }

    [Fact]
    public void TestTryPeekWithNoItems()
    {
        var queue = new TrackQueue();
        Assert.False(queue.TryPeek(out _));
    }
    /// <summary>
    ///     Gets a dummy track used for testing.
    /// </summary>
    /// <returns>a dummy track used for testing</returns>
    private static TrackQueueItem GetDummyTrack()
    {
        var data = new byte[20];
        Random.Shared.NextBytes(data);

        var value = Convert.ToBase64String(data);
        return new(new TrackReference(value));
    }
}