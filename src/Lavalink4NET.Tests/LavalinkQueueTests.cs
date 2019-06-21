namespace Lavalink4NET.Tests
{
    using System;
    using System.Collections.Generic;
    using Lavalink4NET.Player;
    using Xunit;

    public sealed class LavalinkQueueTests
    {
        private static readonly LavalinkTrack DummyTrack = new LavalinkTrack("A", new LavalinkTrackInfo
        {
            Author = "John Doe",
            Source = "http://example.com",
            Title = "My Track",
            TrackIdentifier = "abc"
        });

        private static readonly LavalinkTrack DummyTrack2 = new LavalinkTrack("A", new LavalinkTrackInfo
        {
            Author = "Maria Doe",
            Source = "http://example.org",
            Title = "My Track 2",
            TrackIdentifier = "abcd"
        });

        [Fact]
        public void TestContains()
        {
            var queue = new LavalinkQueue();

            Assert.Empty(queue);
            Assert.DoesNotContain(DummyTrack, queue);

            queue.Add(DummyTrack);

            Assert.Contains(DummyTrack, queue);
        }

        [Fact]
        public void TestDequeue()
        {
            var queue = new LavalinkQueue();

            Assert.Empty(queue);
            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());

            queue.Add(DummyTrack);

            Assert.Equal(DummyTrack, queue.Dequeue());
            Assert.Empty(queue);
        }

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

        [Fact]
        public void TestInsert()
        {
            var queue = new LavalinkQueue();

            Assert.Empty(queue);

            queue.Insert(0, DummyTrack);
            Assert.Equal(0, queue.IndexOf(DummyTrack));
        }

        [Fact]
        public void TestNullAdd()
        {
            var queue = new LavalinkQueue();

            Assert.Empty(queue);
            Assert.Throws<ArgumentNullException>(() => queue.Add(null));
        }

        [Fact]
        public void TestNullContains()
        {
            var queue = new LavalinkQueue();

            Assert.Empty(queue);
            Assert.Throws<ArgumentNullException>(() => queue.Contains(null));
        }

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

        [Fact]
        public void TestQueueEmpty()
        {
            var queue = new LavalinkQueue();

            Assert.Empty(queue);
            Assert.True(queue.IsEmpty);
        }

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

        [Fact]
        public void TestReadableEmpty()
        {
            var queue = new LavalinkQueue();

            Assert.True(queue.IsReadOnly);
        }

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

        [Fact]
        public void TestShuffle()
        {
            var queue = new LavalinkQueue();
            Assert.Empty(queue);

            queue.Add(DummyTrack);
            queue.Shuffle();
            queue.Add(DummyTrack2);
            queue.Add(DummyTrack);
            queue.Add(DummyTrack2);

            Assert.Equal(4, queue.Count);
            Assert.Equal(DummyTrack, queue[0]);
            Assert.Equal(DummyTrack2, queue[1]);
            Assert.Equal(DummyTrack, queue[2]);
            Assert.Equal(DummyTrack2, queue[3]);

            queue.Shuffle();

            Assert.True(queue[0] != DummyTrack
                || queue[1] != DummyTrack2
                || queue[2] != DummyTrack
                || queue[3] != DummyTrack2);
        }

        [Fact]
        public void TestTryDequeue()
        {
            var queue = new LavalinkQueue();

            Assert.Empty(queue);
            Assert.False(queue.TryDequeue(out var track));
            Assert.Equal(default, track);

            queue.Add(DummyTrack);

            Assert.True(queue.TryDequeue(out track));
            Assert.Same(DummyTrack, track);
        }
    }
}