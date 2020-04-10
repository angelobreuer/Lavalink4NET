/*
 *  File:   VoteLavalinkPlayer.cs
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

namespace Lavalink4NET.Player
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Lavalink4NET.Events;

    /// <summary>
    ///     A lavalink player with a queuing and voting system.
    /// </summary>
    public class VoteLavalinkPlayer : QueuedLavalinkPlayer
    {
        private readonly IList<ulong> _skipVotes;

        /// <summary>
        ///     Asynchronously triggered when a track ends.
        /// </summary>
        /// <param name="eventArgs">the track event arguments</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        public override Task OnTrackEndAsync(TrackEndEventArgs eventArgs)
        {
            ClearVotes();
            return base.OnTrackEndAsync(eventArgs);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="VoteLavalinkPlayer"/> class.
        /// </summary>
        public VoteLavalinkPlayer() => _skipVotes = new List<ulong>();

        /// <summary>
        ///     Gets the player skip vote info.
        /// </summary>
        /// <returns>the vote info</returns>
        /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
        public async Task<VoteSkipInfo> GetVoteInfoAsync()
        {
            EnsureNotDestroyed();

            // get users in channel without the bot
            var users = (await Client.GetChannelUsersAsync(GuildId, VoiceChannelId.Value))
                .Where(s => s != Client.CurrentUserId);

            var votes = _skipVotes.Intersect(users).ToArray();
            return new VoteSkipInfo(votes, users.Count());
        }

        /// <summary>
        ///     Clears all user votes.
        /// </summary>
        public virtual void ClearVotes() => _skipVotes.Clear();

        /// <summary>
        ///     Submits an user vote asynchronously.
        /// </summary>
        /// <param name="userId">the user snowflake identifier</param>
        /// <param name="percentage">the minimum voting percentage to skip the track</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
        public virtual async Task<UserVoteSkipInfo> VoteAsync(ulong userId, float percentage = .5f)
        {
            EnsureNotDestroyed();

            var info = await GetVoteInfoAsync();

            if (info.Votes.Contains(userId))
            {
                return new UserVoteSkipInfo(info, false, false);
            }

            // add vote and re-get info, because the votes were changed.
            _skipVotes.Add(userId);
            info = await GetVoteInfoAsync();

            if (info.Percentage >= percentage)
            {
                ClearVotes();
                await SkipAsync();

                return new UserVoteSkipInfo(info, true, true);
            }

            return new UserVoteSkipInfo(info, false, true);
        }
    }
}