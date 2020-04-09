/*
 *  File:   TrackStuckEventArgs.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2020
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */

namespace Lavalink4NET.Events
{
    using System;
    using Player;

    /// <summary>
    ///     The event arguments for the <see cref="LavalinkNode.TrackStuck"/> event.
    /// </summary>
    public class TrackStuckEventArgs
          : TrackEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TrackStuckEventArgs"/> class.
        /// </summary>
        /// <param name="player">the affected player</param>
        /// <param name="trackIdentifier">the identifier of the affected track</param>
        /// <param name="threshold">the threshold in milliseconds</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="player"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="trackIdentifier"/> is <see langword="null"/>.
        /// </exception>
        public TrackStuckEventArgs(LavalinkPlayer player, string trackIdentifier, long threshold)
            : base(player, trackIdentifier)
            => Threshold = threshold;

        /// <summary>
        ///     Gets the threshold in milliseconds.
        /// </summary>
        public long Threshold { get; }
    }
}