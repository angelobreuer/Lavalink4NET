/*
 *  File:   VoteSkipInfo.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2021
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

namespace Lavalink4NET.Player
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Contains information about the current vote information of the player.
    /// </summary>
    public class VoteSkipInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VoteSkipInfo"/> struct.
        /// </summary>
        /// <param name="votes">
        ///     a collection of the snowflake identifier values of the users that voted for skipping
        ///     the current track
        /// </param>
        /// <param name="totalUsers">
        ///     the total number of users in the voice channel (the bot is excluded)
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     the specified <paramref name="votes"/> can not be <see langword="null"/>.
        /// </exception>
        internal VoteSkipInfo(IReadOnlyCollection<ulong> votes, int totalUsers)
        {
            Votes = votes ?? throw new ArgumentNullException(nameof(votes));
            TotalUsers = totalUsers;
        }

        /// <summary>
        ///     Gets the vote percentage in range of 0 - 1f.
        /// </summary>
        public float Percentage => Votes.Count / (float)TotalUsers;

        /// <summary>
        ///     Gets a collection of the snowflake identifier values of the users that voted for
        ///     skipping the current track.
        /// </summary>
        public IReadOnlyCollection<ulong> Votes { get; }

        /// <summary>
        ///     Gets the total number of users in the voice channel (the bot is excluded).
        /// </summary>
        public int TotalUsers { get; }
    }
}
