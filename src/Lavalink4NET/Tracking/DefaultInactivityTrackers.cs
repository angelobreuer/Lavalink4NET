/*
 *  File:   DefaultInactivityTrackers.cs
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

namespace Lavalink4NET.Tracking
{
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    ///     A set of default out-of-box inactivity trackers.
    /// </summary>
    public static class DefaultInactivityTrackers
    {
        /// <summary>
        ///     An inactivity tracker ( <see cref="InactivityTracker"/>) which marks a player as
        ///     "inactive" when there are no users in the channel except the bot itself.
        /// </summary>
        public static InactivityTracker UsersInactivityTracker { get; } = async (player, client) =>
        {
            if (player.VoiceChannelId is null)
            {
                // no users in the voice channel
                return true;
            }

            // count the users in the player voice channel (bot excluded)
            var userCount = (await client.GetChannelUsersAsync(player.GuildId, player.VoiceChannelId.Value))
                .Where(s => s != client.CurrentUserId)
                .Count();

            // check if there are no users in the channel (bot excluded)
            return userCount == 0;
        };

        /// <summary>
        ///     An inactivity tracker ( <see cref="InactivityTracker"/>) which marks a player as
        ///     "inactive" when the player is not playing a track.
        /// </summary>
        public static InactivityTracker ChannelInactivityTracker { get; } = (player, _)
            => Task.FromResult(player.State == Player.PlayerState.NotPlaying);
    }
}
