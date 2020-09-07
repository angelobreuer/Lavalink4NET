/*
 *  File:   PlayerStatus.cs
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

namespace Lavalink4NET.Payloads.Player
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    ///     A wrapper for the player status object.
    /// </summary>
    public readonly struct PlayerStatus
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerStatus"/> class.
        /// </summary>
        /// <param name="time">the time when the update was sent</param>
        /// <param name="position">the track position in milliseconds</param>
        [JsonConstructor]
        public PlayerStatus(long time, int position)
        {
            UpdateTime = DateTimeOffset.FromUnixTimeMilliseconds(time);
            Position = TimeSpan.FromMilliseconds(position);
        }

        /// <summary>
        ///     Gets the track position (at the time the update was received, see: <see cref="UpdateTime"/>).
        /// </summary>
        [JsonIgnore]
        public TimeSpan Position { get; }

        /// <summary>
        ///     Gets the time when the position update was sent.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset UpdateTime { get; }
    }
}
