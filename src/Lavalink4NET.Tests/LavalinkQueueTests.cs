namespace Lavalink4NET.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using Lavalink4NET.Player;
using Xunit;

/// <summary>
///     Contains tests for <see cref="LavalinkQueue"/>.
/// </summary>
public sealed class LavalinkQueueTests
{
    /// <summary>
    ///     A sample dummy track.
    /// </summary>
    private static readonly LavalinkTrack DummyTrack = new LavalinkTrack("A", new LavalinkTrackInfo
    {
        Author = "John Doe",
        SourceName = "http://example.com",
        Title = "My Track",
        TrackIdentifier = "abc"
    });

    /// <summary>
    ///     A sample dummy track.
    /// </summary>
    private static readonly LavalinkTrack DummyTrack2 = new LavalinkTrack("A", new LavalinkTrackInfo
    {
        Author = "Maria Doe",
        SourceName = "http://example.org",
        Title = "My Track 2",
        TrackIdentifier = "abcd"
    });

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.AddRange(IEnumerable{LavalinkTrack})"/>
    /// </summary>
    [Fact]
    public void TestAddRange()
    {
        var queue = new LavalinkQueue();
        Assert.Empty(queue);

        queue.AddRange(new List<LavalinkTrack> { DummyTrack, DummyTrack2 });

        Assert.NotEmpty(queue);
        Assert.Same(DummyTrack, queue[0]);
        Assert.Same(DummyTrack2, queue[1]);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.Contains(LavalinkTrack)"/>.
    /// </summary>
    [Fact]
    public void TestContains()
    {
        var queue = new LavalinkQueue();

        Assert.Empty(queue);
        Assert.DoesNotContain(DummyTrack, queue);

        queue.Add(DummyTrack);

        Assert.Contains(DummyTrack, queue);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.Dequeue"/>.
    /// </summary>
    [Fact]
    public void TestDequeue()
    {
        var queue = new LavalinkQueue();

        Assert.Empty(queue);
        Assert.Throws<InvalidOperationException>(() => queue.Dequeue());

        queue.Add(DummyTrack);
        queue.Add(DummyTrack2);

        Assert.Equal(DummyTrack, queue.Dequeue());
        Assert.Single(queue);

        Assert.Equal(DummyTrack2, queue.Dequeue());
        Assert.Empty(queue);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.Distinct"/>.
    /// </summary>
    [Fact]
    public void TestDistinct()
    {
        var queue = new LavalinkQueue();
        Assert.Empty(queue);

        queue.Distinct();

        Assert.Empty(queue);

        queue.Add(DummyTrack);
        queue.Add(DummyTrack);
        queue.Add(DummyTrack);
        queue.Add(DummyTrack);

        Assert.Equal(4, queue.Count);

        queue.Distinct();

        Assert.Single(queue);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.IndexOf(LavalinkTrack)"/>.
    /// </summary>
    [Fact]
    public void TestIndexOf()
    {
        var queue = new LavalinkQueue();

        Assert.Empty(queue);
        Assert.Throws<ArgumentNullException>(() => queue.IndexOf(null));
        Assert.Equal(-1, queue.IndexOf(DummyTrack));

        queue.Add(DummyTrack);

        Assert.Equal(0, queue.IndexOf(DummyTrack));
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.Insert(int, LavalinkTrack)"/>.
    /// </summary>
    [Fact]
    public void TestInsert()
    {
        var queue = new LavalinkQueue();

        Assert.Empty(queue);

        queue.Insert(0, DummyTrack);
        Assert.Equal(0, queue.IndexOf(DummyTrack));
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.Add(LavalinkTrack)"/>.
    /// </summary>
    [Fact]
    public void TestNullAdd()
    {
        var queue = new LavalinkQueue();

        Assert.Empty(queue);
        Assert.Throws<ArgumentNullException>(() => queue.Add(null));
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.Contains(LavalinkTrack)"/>.
    /// </summary>
    [Fact]
    public void TestNullContains()
    {
        var queue = new LavalinkQueue();

        Assert.Empty(queue);
        Assert.Throws<ArgumentNullException>(() => queue.Contains(null));
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.Add(LavalinkTrack)"/>.
    /// </summary>
    [Fact]
    public void TestQueueAdd()
    {
        var queue = new LavalinkQueue();

        Assert.Empty(queue);

        queue.Add(DummyTrack);

        Assert.NotEmpty(queue);
        Assert.Single(queue);

        queue.Add(DummyTrack);

        Assert.Equal(2, queue.Count);

        queue.Clear();

        Assert.Empty(queue);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.Clear"/>.
    /// </summary>
    [Fact]
    public void TestQueueClear()
    {
        var queue = new LavalinkQueue();

        Assert.Empty(queue);

        queue.Add(DummyTrack);

        Assert.NotEmpty(queue);
        Assert.Equal(1, queue.Clear());
        Assert.Empty(queue);

        queue.Add(DummyTrack);

        Assert.NotEmpty(queue);

        ((ICollection<LavalinkTrack>)queue).Clear();

        Assert.Empty(queue);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.IsEmpty"/>.
    /// </summary>
    [Fact]
    public void TestQueueEmpty()
    {
        var queue = new LavalinkQueue();

        Assert.Empty(queue);
        Assert.True(queue.IsEmpty);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.IndexOf(LavalinkTrack)"/>.
    /// </summary>
    [Fact]
    public void TestQueueIndexGetSet()
    {
        var queue = new LavalinkQueue();

        Assert.Empty(queue);
        Assert.Throws<ArgumentOutOfRangeException>(() => queue[0]);
        Assert.Throws<ArgumentOutOfRangeException>(() => queue[0] = DummyTrack);

        queue.Add(DummyTrack);

        Assert.Same(DummyTrack, queue[0]);
        Assert.Throws<ArgumentNullException>(() => queue[0] = null);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.Tracks"/>.
    /// </summary>
    [Fact]
    public void TestQueueTracksGet()
    {
        var queue = new LavalinkQueue();
        var tracks = new[] { DummyTrack };

        Assert.Empty(queue);

        queue.Tracks = tracks;

        Assert.NotEmpty(queue);

        Assert.Equal(queue.Tracks, tracks);

        // should be a copy of the tracks
        Assert.NotSame(queue.Tracks, tracks);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.Tracks"/>.
    /// </summary>
    [Fact]
    public void TestQueueTracksSet()
    {
        var queue = new LavalinkQueue();

        Assert.Empty(queue);

        queue.Tracks = new[] { DummyTrack };

        Assert.NotEmpty(queue);

        queue.Tracks = new LavalinkTrack[0];

        Assert.Empty(queue);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.IsReadOnly"/>.
    /// </summary>
    [Fact]
    public void TestReadableEmpty()
    {
        var queue = new LavalinkQueue();

        Assert.False(queue.IsReadOnly);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.Remove(LavalinkTrack)"/>.
    /// </summary>
    [Fact]
    public void TestRemove()
    {
        var queue = new LavalinkQueue();
        Assert.Empty(queue);

        queue.Add(DummyTrack);

        Assert.Single(queue);
        Assert.True(queue.Remove(DummyTrack));
        Assert.Empty(queue);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.RemoveAll(Predicate{LavalinkTrack})"/>.
    /// </summary>
    [Fact]
    public void TestRemoveAll()
    {
        var queue = new LavalinkQueue();
        Assert.Empty(queue);

        queue.Add(DummyTrack);
        queue.Add(DummyTrack2);

        Assert.Equal(2, queue.Count);
        Assert.Equal(1, queue.RemoveAll(track => track == DummyTrack2));
        Assert.Equal(1, queue.RemoveAll(trakc => true));
        Assert.Empty(queue);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.RemoveAt(int)"/>.
    /// </summary>
    [Fact]
    public void TestRemoveAt()
    {
        var queue = new LavalinkQueue();
        Assert.Empty(queue);

        queue.Add(DummyTrack);
        queue.Add(DummyTrack);

        Assert.Equal(2, queue.Count);

        queue.RemoveAt(0);

        Assert.Single(queue);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.RemoveRange(int, int)"/>
    /// </summary>
    [Fact]
    public void TestRemoveRange()
    {
        var queue = new LavalinkQueue();

        Assert.Empty(queue);

        queue.Add(DummyTrack);
        queue.Add(DummyTrack);
        queue.Add(DummyTrack2);
        queue.Add(DummyTrack2);

        Assert.NotEmpty(queue);

        queue.RemoveRange(1, 2);

        Assert.NotEmpty(queue);
        Assert.Same(DummyTrack, queue[0]);
        Assert.Same(DummyTrack2, queue[1]);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.Shuffle"/>.
    /// </summary>
    [Fact]
    public void TestShuffle()
    {
        var queue = new LavalinkQueue();
        Assert.Empty(queue);

        for (var i = 0; i < 10; i++)
        {
            queue.Add(DummyTrack);
            queue.Add(DummyTrack2);
        }

        Assert.Equal(20, queue.Count);

        var originalQueue = queue.ToArray();

        queue.Shuffle();

        Assert.NotEqual(queue.ToArray(), originalQueue);
    }

    /// <summary>
    ///     Tests <see cref="LavalinkQueue.TryDequeue(out LavalinkTrack)"/>.
    /// </summary>
    [Fact]
    public void TestTryDequeue()
    {
        var queue = new LavalinkQueue();

        Assert.Empty(queue);
        Assert.False(queue.TryDequeue(out var track));
        Assert.Equal(default, track);

        queue.Add(DummyTrack);
        queue.Add(DummyTrack2);

        Assert.True(queue.TryDequeue(out track));
        Assert.Same(DummyTrack, track);

        Assert.Single(queue);

        Assert.True(queue.TryDequeue(out track));
        Assert.Same(DummyTrack2, track);

        Assert.Empty(queue);
        Assert.False(queue.TryDequeue(out track));
        Assert.Equal(default, track);
    }
}
