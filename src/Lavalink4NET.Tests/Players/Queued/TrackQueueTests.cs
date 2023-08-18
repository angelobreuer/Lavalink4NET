namespace Lavalink4NET.Tests.Players.Queued;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Xunit;

public sealed class TrackQueueTests
{
    [Fact]
    public void TestConstructorHistoryCapacity()
    {
        var queue = new TrackQueue(8);

        Assert.NotNull(queue.History);
        Assert.True(queue.HasHistory);
    }

    [Fact]
    public void TestConstructorNoHistory()
    {
        var queue = new TrackQueue(null);

        Assert.Null(queue.History);
        Assert.False(queue.HasHistory);
    }

    [Fact]
    public void TestInsert()
    {
        var queue = new TrackQueue(null);
        var item1 = new TestTrackQueueItem("a");
        var item2 = new TestTrackQueueItem("b");

        queue.Insert(0, item1);
        queue.Insert(0, item2);

        Assert.Equal(2, queue.Count);
        Assert.Equal(item2, queue[0]);
        Assert.Equal(item1, queue[1]);
    }

    [Fact]
    public void TestInsertRange()
    {
        var queue = new TrackQueue(null);
        var items = new List<ITrackQueueItem>
            {
                new TestTrackQueueItem("a"),
                new TestTrackQueueItem("b"),
            };

        queue.InsertRange(0, items);

        Assert.Equal(2, queue.Count);
        Assert.Equal(items[0], queue[0]);
        Assert.Equal(items[1], queue[1]);
    }

    [Fact]
    public async Task TestInsertAsync()
    {
        var queue = new TrackQueue(null) as ITrackQueue;
        var item1 = new TestTrackQueueItem("a");
        var item2 = new TestTrackQueueItem("b");

        await queue.InsertAsync(0, item1);
        await queue.InsertAsync(0, item2);

        Assert.Equal(2, queue.Count);
        Assert.Equal(item2, queue[0]);
        Assert.Equal(item1, queue[1]);
    }

    [Fact]
    public async Task TestInsertRangeAsync()
    {
        var queue = new TrackQueue(null) as ITrackQueue;
        var items = new List<ITrackQueueItem>
            {
                new TestTrackQueueItem("a"),
                new TestTrackQueueItem("b"),
            };

        await queue.InsertRangeAsync(0, items);

        Assert.Equal(2, queue.Count);
        Assert.Equal(items[0], queue[0]);
        Assert.Equal(items[1], queue[1]);
    }

    [Fact]
    public void TestShuffle()
    {
        var queue = new TrackQueue(null);
        var originalItems = Enumerable.Range(1, 500).Select(value => new TestTrackQueueItem($"{value}")).ToList();

        queue.AddRange(originalItems);
        queue.Shuffle();

        Assert.False(originalItems.SequenceEqual(queue));
    }

    [Fact]
    public void TestPeek()
    {
        var queue = new TrackQueue(null);
        queue.AddRange(Enumerable.Range(1, 3).Select(value => new TestTrackQueueItem($"{value}")).ToList());

        var peekedItem = queue.Peek();

        Assert.NotNull(peekedItem);
    }

    [Fact]
    public void TestTryPeekTrue()
    {
        var queue = new TrackQueue(null)
        {
            new TestTrackQueueItem("a")
        };

        var success = queue.TryPeek(out var peekedItem);

        Assert.True(success);
        Assert.NotNull(peekedItem);
    }

    [Fact]
    public void TestTryPeekFalse()
    {
        var queue = new TrackQueue(null);

        var success = queue.TryPeek(out var peekedItem);

        Assert.False(success);
        Assert.Null(peekedItem);
    }
}

file sealed record class TestTrackQueueItem(string Id) : ITrackQueueItem
{
    public TrackReference Reference => new(Id);
}
