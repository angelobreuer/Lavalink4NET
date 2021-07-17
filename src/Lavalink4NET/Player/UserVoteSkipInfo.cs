/*
 *  File:   UserVoteSkipInfo.cs
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
    using System.Collections.Generic;

    /// <summary>
    ///     Contains information about the current vote information and the submitted user vote.
    /// </summary>
    public sealed class UserVoteSkipInfo : VoteSkipInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UserVoteSkipInfo"/> class.
        /// </summary>
        /// <param name="info">the <see cref="VoteSkipInfo"/> to clone</param>
        /// <param name="wasSkipped">
        ///     a value indicating whether the user vote submit caused that the current playing track
        ///     was skipped
        /// </param>
        /// <param name="wasAdded">
        ///     a value indicating whether the user vote submit was added to the vote list
        /// </param>
        public UserVoteSkipInfo(VoteSkipInfo info, bool wasSkipped, bool wasAdded)
            : this(info.Votes, info.TotalUsers, wasSkipped, wasAdded)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserVoteSkipInfo"/> class.
        /// </summary>
        /// <param name="votes">
        ///     a collection of the snowflake identifier values of the users that voted for skipping
        ///     the current track
        /// </param>
        /// <param name="totalUsers">
        ///     the total number of users in the voice channel (the bot is excluded)
        /// </param>
        /// <param name="wasSkipped">
        ///     a value indicating whether the user vote submit caused that the current playing track
        ///     was skipped
        /// </param>
        /// <param name="wasAdded">
        ///     a value indicating whether the user vote submit was added to the vote list
        /// </param>
        public UserVoteSkipInfo(IReadOnlyCollection<ulong> votes, int totalUsers, bool wasSkipped, bool wasAdded)
            : base(votes, totalUsers)
        {
            WasSkipped = wasSkipped;
            WasAdded = wasAdded;
        }

        /// <summary>
        ///     Gets a value indicating whether the user vote submit caused that the current playing
        ///     track was skipped.
        /// </summary>
        public bool WasSkipped { get; }

        /// <summary>
        ///     Gets a value indicating whether the user vote submit was added to the vote list.
        /// </summary>
        public bool WasAdded { get; }
    }
}