/*
 *  File:   TrackExceptionEventArgs.cs
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
    using Lavalink4NET.Rest;
    using Player;

    /// <summary>
    ///     The event arguments for the <see cref="LavalinkNode.TrackException"/> event.
    /// </summary>
    public class TrackExceptionEventArgs
         : TrackEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TrackExceptionEventArgs"/> class.
        /// </summary>
        /// <param name="player">the affected player</param>
        /// <param name="trackIdentifier">the identifier of the affected track</param>
        /// <param name="exception">the error that occurred</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="player"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="trackIdentifier"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="exception"/> is <see langword="null"/>.
        /// </exception>
        public TrackExceptionEventArgs(LavalinkPlayer player, string trackIdentifier, TrackException exception)
            : base(player, trackIdentifier)
            => Exception = exception ?? throw new ArgumentNullException(nameof(exception));

        /// <summary>
        ///     Gets the error that occurred.
        /// </summary>
        public TrackException Exception { get; }
    }
}
