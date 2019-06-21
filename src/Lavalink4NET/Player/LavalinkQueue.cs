namespace Lavalink4NET.Player
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     A thread-safe queue for <see cref="LavalinkTrack"/>.
    /// </summary>
    public sealed class LavalinkQueue : IList<LavalinkTrack>
    {
        private readonly List<LavalinkTrack> _list;
        private readonly object _syncRoot;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LavalinkQueue"/> class.
        /// </summary>
        public LavalinkQueue()
        {
            _list = new List<LavalinkTrack>();
            _syncRoot = new object();
        }

        /// <summary>
        ///     Gets the number of queued tracks.
        /// </summary>
        /// <remarks>
        ///     This property is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    return _list.Count;
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the queue is empty.
        /// </summary>
        /// <remarks>
        ///     This property is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        public bool IsEmpty => Count == 0;

        /// <summary>
        ///     Gets a value indicating whether the queue is read-only.
        /// </summary>
        /// <remarks>
        ///     This property is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        public bool IsReadOnly => true;

        /// <summary>
        ///     Gets or sets the enqueued tracks.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        public IReadOnlyList<LavalinkTrack> Tracks
        {
            get
            {
                lock (_syncRoot)
                {
                    // return new array
                    return _list.ToArray();
                }
            }

            set
            {
                lock (_syncRoot)
                {
                    // clear old tracks
                    _list.Clear();

                    // add new tracks
                    _list.AddRange(value);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the track at the specified <paramref name="index"/>.
        /// </summary>
        /// <remarks>
        ///     This indexer property is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        /// <param name="index">the zero-based position</param>
        /// <returns>the track at the specified <paramref name="index"/></returns>
        public LavalinkTrack this[int index]
        {
            get
            {
                lock (_syncRoot)
                {
                    return _list[index];
                }
            }

            set
            {
                // a track can not be null
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                lock (_syncRoot)
                {
                    _list[index] = value;
                }
            }
        }

        /// <summary>
        ///     Adds a track at the end of the queue.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        /// <param name="track">the track to add</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="track"/> is <see langword="null"/>.
        /// </exception>
        public void Add(LavalinkTrack track)
        {
            if (track is null)
            {
                throw new ArgumentNullException(nameof(track));
            }

            lock (_syncRoot)
            {
                _list.Add(track);
            }
        }

        /// <summary>
        ///     Clears all tracks from the queue.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        /// <returns>the number of tracks removed</returns>
        public int Clear()
        {
            lock (_syncRoot)
            {
                var tracks = _list.Count;
                _list.Clear();
                return tracks;
            }
        }

        /// <summary>
        ///     Clears the queue.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        void ICollection<LavalinkTrack>.Clear()
        {
            lock (_syncRoot)
            {
                _list.Clear();
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the specified <paramref name="track"/> is in the queue.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        /// <param name="track">the track to find</param>
        /// <returns>
        ///     a value indicating whether the specified <paramref name="track"/> is in the queue
        /// </returns>
        public bool Contains(LavalinkTrack track)
        {
            if (track is null)
            {
                throw new ArgumentNullException(nameof(track));
            }

            lock (_syncRoot)
            {
                return _list.Contains(track);
            }
        }

        /// <summary>
        ///     Copies all tracks to the specified <paramref name="array"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        /// <param name="array">the array to the tracks to</param>
        /// <param name="index">the zero-based writing start index</param>
        public void CopyTo(LavalinkTrack[] array, int index)
        {
            lock (_syncRoot)
            {
                _list.CopyTo(array, index);
            }
        }

        /// <summary>
        ///     Dequeues a track using the FIFO method.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        /// <returns>the dequeued track</returns>
        /// <exception cref="InvalidOperationException">
        ///     thrown if no tracks were in the queue
        /// </exception>
        public LavalinkTrack Dequeue()
        {
            lock (_syncRoot)
            {
                if (_list.Count <= 0)
                {
                    throw new InvalidOperationException("No tracks in to dequeue.");
                }

                var track = _list[Count - 1];
                _list.RemoveAt(Count - 1);
                return track;
            }
        }

        /// <summary>
        ///     Deletes all duplicate tracks from the queue.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        public void Distinct()
        {
            lock (_syncRoot)
            {
                // no distinct needed, only a single track enqueued (there can not be any duplicate
                // tracks in queue)
                if (_list.Count <= 1)
                {
                    return;
                }

                // distinct tracks
                var tracks = _list.GroupBy(track => track.Identifier)
                    .Select(s => s.First())
                    .ToArray();

                // clear old tracks
                _list.Clear();

                // add distinct tracks
                _list.AddRange(tracks);
            }
        }

        /// <summary>
        ///     Gets the track enumerator.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        /// <returns>the track enumerator</returns>
        public IEnumerator<LavalinkTrack> GetEnumerator()
        {
            lock (_syncRoot)
            {
                return _list.ToList().GetEnumerator();
            }
        }

        /// <summary>
        ///     Gets the track enumerator.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        /// <returns>the track enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_syncRoot)
            {
                return _list.ToArray().GetEnumerator();
            }
        }

        /// <summary>
        ///     Gets the zero-based index of the specified <paramref name="track"/>.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        /// <param name="track">the track to locate</param>
        /// <returns>the zero-based index of the specified <paramref name="track"/></returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="track"/> is <see langword="null"/>.
        /// </exception>
        public int IndexOf(LavalinkTrack track)
        {
            if (track is null)
            {
                throw new ArgumentNullException(nameof(track));
            }

            lock (_syncRoot)
            {
                return _list.IndexOf(track);
            }
        }

        /// <summary>
        ///     Inserts the specified <paramref name="track"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        /// <param name="index">the zero-based index to insert (e.g. 0 = top)</param>
        /// <param name="track">the track to insert</param>
        public void Insert(int index, LavalinkTrack track)
        {
            lock (_syncRoot)
            {
                _list.Insert(index, track);
            }
        }

        /// <summary>
        ///     Tries to remove the specified <paramref name="track"/> from the queue.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        /// <param name="track">the track to remove</param>
        /// <returns>
        ///     a value indicating whether the track was found and removed from the queue
        /// </returns>
        public bool Remove(LavalinkTrack track)
        {
            lock (_syncRoot)
            {
                return _list.Remove(track);
            }
        }

        /// <summary>
        ///     Removes all tracks that matches the specified <paramref name="predicate"/>.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        /// <param name="predicate">the track predicate</param>
        /// <returns>the number of tracks removed</returns>
        public int RemoveAll(Predicate<LavalinkTrack> predicate)
        {
            lock (_syncRoot)
            {
                return _list.RemoveAll(predicate);
            }
        }

        /// <summary>
        ///     Removes a track at the specified <paramref name="index"/>.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        /// <param name="index">the index to remove the track</param>
        public void RemoveAt(int index)
        {
            lock (_syncRoot)
            {
                _list.RemoveAt(index);
            }
        }

        /// <summary>
        ///     Shuffles / mixes all tracks in the queue.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        public void Shuffle()
        {
            lock (_syncRoot)
            {
                // no shuffling needed
                if (_list.Count <= 2)
                {
                    return;
                }

                // shuffle tracks
                var tracks = _list.OrderBy(s => Guid.NewGuid()).ToArray();

                // clear old tracks
                _list.Clear();

                // add shuffled tracks
                _list.AddRange(tracks);
            }
        }

        /// <summary>
        ///     Tries to dequeue a track using the FIFO method.
        /// </summary>
        /// <remarks>
        ///     This method is thread-safe, so it can be used from multiple threads at once safely.
        /// </remarks>
        /// <param name="track">the dequeued track; or default is the result is <see langword="false"/>.</param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if no tracks were in the queue
        /// </exception>
        /// <returns>a value indicating whether a track was dequeued.</returns>
        public bool TryDequeue(out LavalinkTrack track)
        {
            lock (_syncRoot)
            {
                if (_list.Count <= 0)
                {
                    track = default;
                    return false;
                }

                track = _list[Count - 1];
                _list.RemoveAt(Count - 1);
                return true;
            }
        }
    }
}