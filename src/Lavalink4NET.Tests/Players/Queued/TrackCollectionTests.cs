namespace Lavalink4NET.Tests.Players.Queued;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Xunit;

public sealed class TrackCollectionTests
{
    [Fact]
    public void TestIsEmptyFalse()
    {
        var collection = new TestTrackCollection
        {
            new TestTrackQueueItem()
        };

        Assert.False(collection.IsEmpty);
    }

    [Fact]
    public void TestIsEmptyTrue()
    {
        var collection = new TestTrackCollection();

        Assert.True(collection.IsEmpty);
    }

    [Fact]
    public void TestAdd()
    {
        var collection = new TestTrackCollection();
        var item = new TestTrackQueueItem();

        collection.Add(item);

        Assert.Single(collection);
        Assert.Equal(item, collection[0]);
    }

    [Fact]
    public void TestAddRange()
    {
        var collection = new TestTrackCollection();
        var items = Enumerable.Range(1, 3).Select(_ => new TestTrackQueueItem()).ToList();

        collection.AddRange(items);

        Assert.Equal(3, collection.Count);
        Assert.Equal(items, collection);
    }

    [Fact]
    public void TestClear()
    {
        var collection = new TestTrackCollection();
        collection.AddRange(Enumerable.Range(1, 5).Select(_ => new TestTrackQueueItem()).ToList());

        collection.Clear();

        Assert.Empty(collection);
    }

    [Fact]
    public void TestContains()
    {
        var collection = new TestTrackCollection();
        var item = new TestTrackQueueItem();
        collection.Add(item);

        var containsItem = collection.Contains(item);
        var notContainsItem = collection.Contains(new TestTrackQueueItem2());

        Assert.True(containsItem);
        Assert.False(notContainsItem);
    }

    [Fact]
    public void TestDistinct()
    {
        var collection = new TestTrackCollection();
        var item1 = new TestTrackQueueItem();
        var item2 = new TestTrackQueueItem2();
        collection.AddRange(new List<ITrackQueueItem> { item1, item2, item1 });

        var difference = collection.Distinct(null);

        Assert.Equal(1, difference);
        Assert.Equal(2, collection.Count);
    }

    [Fact]
    public void TestRemove()
    {
        var collection = new TestTrackCollection();
        var item = new TestTrackQueueItem();
        collection.Add(item);

        var removed = collection.Remove(item);
        var stillContainsItem = collection.Contains(item);

        Assert.True(removed);
        Assert.False(stillContainsItem);
    }

    [Fact]
    public void TestRemoveAt()
    {
        var collection = new TestTrackCollection();
        collection.AddRange(Enumerable.Range(1, 5).Select(_ => new TestTrackQueueItem()).ToList());

        var removed = collection.RemoveAt(2);

        Assert.True(removed);
        Assert.Equal(4, collection.Count);
    }

    [Fact]
    public void TestRemoveAtFalseIfOutOfRange()
    {
        var collection = new TestTrackCollection();
        collection.AddRange(Enumerable.Range(1, 5).Select(_ => new TestTrackQueueItem()).ToList());

        var removed = collection.RemoveAt(5);

        Assert.False(removed);
        Assert.Equal(5, collection.Count);
    }

    [Fact]
    public void TestRemoveAll()
    {
        var collection = new TestTrackCollection();
        var items = Enumerable.Range(1, 5).Select(_ => new TestTrackQueueItem()).ToList();
        collection.AddRange(items);

        var index = 0;
        var removedCount = collection.RemoveAll(item => ++index % 2 is 0);

        Assert.Equal(2, removedCount);
        Assert.Equal(3, collection.Count);
    }

    [Fact]
    public void TestRemoveRange()
    {
        var collection = new TestTrackCollection();
        collection.AddRange(Enumerable.Range(1, 5).Select(_ => new TestTrackQueueItem()).ToList());

        collection.RemoveRange(2, 2);

        Assert.Equal(3, collection.Count);
    }

    [Fact]
    public async Task TestAddAsync()
    {
        var collection = new TestTrackCollection() as ITrackCollection;
        var item = new TestTrackQueueItem();

        await collection
            .AddAsync(item)
            .ConfigureAwait(false);

        Assert.Single(collection);
        Assert.Equal(item, collection[0]);
    }

    [Fact]
    public async Task TestAddRangeAsync()
    {
        var collection = new TestTrackCollection() as ITrackCollection;
        var items = Enumerable.Range(1, 3).Select(_ => new TestTrackQueueItem()).ToList();

        await collection
            .AddRangeAsync(items)
            .ConfigureAwait(false);

        Assert.Equal(3, collection.Count);
        Assert.Equal(items, collection);
    }

    [Fact]
    public async Task TestClearAsync()
    {
        var collection = new TestTrackCollection() as ITrackCollection;

        await collection
            .AddRangeAsync(Enumerable.Range(1, 5).Select(_ => new TestTrackQueueItem()).ToList())
            .ConfigureAwait(false);

        await collection
            .ClearAsync()
            .ConfigureAwait(false);

        Assert.Empty(collection);
    }

    [Fact]
    public async Task TestDistinctAsync()
    {
        var collection = new TestTrackCollection() as ITrackCollection;

        var item1 = new TestTrackQueueItem();
        var item2 = new TestTrackQueueItem2();

        await collection
            .AddRangeAsync(new List<ITrackQueueItem> { item1, item2, item1 })
            .ConfigureAwait(false);

        var difference = await collection
            .DistinctAsync(null)
            .ConfigureAwait(false);

        Assert.Equal(1, difference);
        Assert.Equal(2, collection.Count);
    }

    [Fact]
    public async Task TestRemoveAsync()
    {
        var collection = new TestTrackCollection() as ITrackCollection;

        var item = new TestTrackQueueItem();

        await collection
            .AddAsync(item)
            .ConfigureAwait(false);

        var removed = await collection
            .RemoveAsync(item)
            .ConfigureAwait(false);
        var stillContainsItem = collection.Contains(item);

        Assert.True(removed);
        Assert.False(stillContainsItem);
    }

    [Fact]
    public async Task TestRemoveAtAsync()
    {
        var collection = new TestTrackCollection() as ITrackCollection;

        await collection
            .AddRangeAsync(Enumerable.Range(1, 5).Select(_ => new TestTrackQueueItem()).ToList())
            .ConfigureAwait(false);

        var removed = await collection
            .RemoveAtAsync(2)
            .ConfigureAwait(false);

        Assert.True(removed);
        Assert.Equal(4, collection.Count);
    }

    [Fact]
    public async Task TestRemoveAtFalseIfOutOfRangeAsync()
    {
        var collection = new TestTrackCollection() as ITrackCollection;

        await collection
            .AddRangeAsync(Enumerable.Range(1, 5).Select(_ => new TestTrackQueueItem()).ToList())
            .ConfigureAwait(false);

        var removed = await collection
            .RemoveAtAsync(5)
            .ConfigureAwait(false);

        Assert.False(removed);
        Assert.Equal(5, collection.Count);
    }

    [Fact]
    public async Task TestRemoveAllAsync()
    {
        var collection = new TestTrackCollection() as ITrackCollection;

        var items = Enumerable.Range(1, 5).Select(_ => new TestTrackQueueItem()).ToList();

        await collection
            .AddRangeAsync(items)
            .ConfigureAwait(false);

        var index = 0;
        var removedCount = await collection
            .RemoveAllAsync(item => ++index % 2 is 0)
            .ConfigureAwait(false);

        Assert.Equal(2, removedCount);
        Assert.Equal(3, collection.Count);
    }

    [Fact]
    public async Task TestRemoveRangeAsync()
    {
        var collection = new TestTrackCollection() as ITrackCollection;

        await collection
            .AddRangeAsync(Enumerable.Range(1, 5).Select(_ => new TestTrackQueueItem()).ToList())
            .ConfigureAwait(false);

        await collection
            .RemoveRangeAsync(2, 2)
            .ConfigureAwait(false);

        Assert.Equal(3, collection.Count);
    }
}

file sealed class TestTrackCollection : TrackCollection
{
}

file sealed record class TestTrackQueueItem : ITrackQueueItem
{
    public TrackReference Track => new TrackReference("track1");
}


file sealed record class TestTrackQueueItem2 : ITrackQueueItem
{
    public TrackReference Track => new TrackReference("track2");
}
