namespace Lavalink4NET.Tests.Players.Queued;

using System.Collections.Generic;
using System.Linq;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Xunit;

public sealed class TrackHistoryTests
{
    [Fact]
    public void TestAddNoCapacity()
    {
        var history = new TrackHistory(null);
        var item = new TestTrackQueueItem("abc");

        history.Add(item);

        Assert.Single(history);
        Assert.Equal(item, history[0]);
    }

    [Fact]
    public void TestAddWithCapacityNotExceeded()
    {
        var history = new TrackHistory(3);
        history.AddRange(GenerateItems(1, 2));

        var item = new TestTrackQueueItem("abc");
        history.Add(item);

        Assert.Equal(3, history.Count);
        Assert.Equal(item, history[2]);
    }

    [Fact]
    public void TestAddWithCapacityExceeded()
    {
        var history = new TrackHistory(2);
        history.AddRange(GenerateItems(1, 2));

        var item = new TestTrackQueueItem("abc");
        history.Add(item);

        Assert.Equal(2, history.Count);
        Assert.Equal(item, history[1]);
    }

    [Fact]
    public void TestAddRangeNoCapacity()
    {
        var history = new TrackHistory(null);
        var items = GenerateItems(1, 3);

        history.AddRange(items);

        Assert.Equal(3, history.Count);
        Assert.Equal(items, history);
    }

    [Fact]
    public void TestAddRangeWithCapacityNotExceeded()
    {
        var history = new TrackHistory(5);
        history.AddRange(GenerateItems(1, 2));

        var items = GenerateItems(3, 3);
        history.AddRange(items);

        Assert.Equal(5, history.Count);
        Assert.Equal(items, history.Skip(2));
    }

    [Fact]
    public void TestAddRangeWithCapacityExceeded()
    {
        var history = new TrackHistory(4);

        var firstItems = GenerateItems(1, 2);
        var secondItems = GenerateItems(3, 3);

        history.AddRange(firstItems);
        history.AddRange(secondItems);

        Assert.Equal(4, history.Count);
        Assert.Equal(firstItems.Skip(1), history.Take(1));
        Assert.Equal(secondItems, history.Skip(1));
    }

    [Fact]
    public void TestRemoveAtWithCapacityNotExceeded()
    {
        var history = new TrackHistory(5);
        history.AddRange(GenerateItems(1, 5));

        history.RemoveAt(2);

        Assert.Equal(4, history.Count);
    }

    [Fact]
    public void TestRemoveAtWithCapacityExceeded()
    {
        var history = new TrackHistory(4);
        history.AddRange(GenerateItems(1, 5));

        history.RemoveAt(2);

        Assert.Equal(3, history.Count);
    }

    private static IReadOnlyList<ITrackQueueItem> GenerateItems(int offset, int count)
    {
        return Enumerable
            .Range(offset, count)
            .Select(id => new TestTrackQueueItem($"track-{id}"))
            .ToList();
    }
}

file sealed record class TestTrackQueueItem(string Id) : ITrackQueueItem
{
    public TrackReference Track => new TrackReference(Id);
}
