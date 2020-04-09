/*
 *  File:   IAudioServiceExtensions.cs
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

namespace Lavalink4NET.DiscordNet
{
    using System.Threading.Tasks;
    using Discord;
    using Lavalink4NET.Player;

    /// <summary>
    ///     A set of different extension methods for the <see cref="IAudioService"/> class.
    /// </summary>
    public static class IAudioServiceExtensions
    {
        /// <summary>
        ///     Gets the audio player for the specified <paramref name="guild"/>.
        /// </summary>
        /// <typeparam name="TPlayer">the type of the player to use</typeparam>
        /// <param name="audioService">the audio service</param>
        /// <param name="guild">the guild to get the player for</param>
        /// <returns>the player for the guild</returns>
        public static TPlayer GetPlayer<TPlayer>(this IAudioService audioService, IGuild guild) where TPlayer : LavalinkPlayer
            => audioService.GetPlayer<TPlayer>(guild.Id);

        /// <summary>
        ///     Gets the audio player for the specified <paramref name="guild"/>.
        /// </summary>
        /// <param name="audioService">the audio service</param>
        /// <param name="guild">the guild to get the player for</param>
        /// <returns>the player for the guild</returns>
        public static LavalinkPlayer GetPlayer(this IAudioService audioService, IGuild guild)
            => audioService.GetPlayer(guild.Id);

        /// <summary>
        ///     Gets a value indicating whether a player is created for the specified <paramref name="guild"/>.
        /// </summary>
        /// <param name="audioService">the audio service</param>
        /// <param name="guild">the guild to create the player for</param>
        /// <returns>a value indicating whether a player is created for the specified <paramref name="guild"/></returns>
        public static bool HasPlayer(this IAudioService audioService, IGuild guild)
            => audioService.HasPlayer(guild.Id);

        /// <summary>
        ///     Joins the specified <paramref name="audioService"/> asynchronously.
        /// </summary>
        /// <typeparam name="TPlayer">the type of the player to create</typeparam>
        /// <param name="audioService">the audio service</param>
        /// <param name="voiceChannel">the voice channel to join</param>
        /// <param name="selfDeaf">a value indicating whether the bot user should be self deafened</param>
        /// <param name="selfMute">a value indicating whether the bot user should be self muted</param>
        /// <returns>
        ///     a task that represents the asynchronous operation
        ///     <para>the audio player</para>
        /// </returns>
        public static Task<TPlayer> JoinAsync<TPlayer>(this IAudioService audioService, IVoiceChannel voiceChannel,
            bool selfDeaf = false, bool selfMute = false) where TPlayer : LavalinkPlayer
            => audioService.JoinAsync<TPlayer>(voiceChannel.GuildId, voiceChannel.Id, selfDeaf, selfMute);

        /// <summary>
        ///     Joins the specified <paramref name="audioService"/> asynchronously.
        /// </summary>
        /// <param name="audioService">the audio service</param>
        /// <param name="voiceChannel">the voice channel to join</param>
        /// <param name="selfDeaf">a value indicating whether the bot user should be self deafened</param>
        /// <param name="selfMute">a value indicating whether the bot user should be self muted</param>
        /// <returns>
        ///     a task that represents the asynchronous operation
        ///     <para>the audio player</para>
        /// </returns>
        public static Task JoinAsync(this IAudioService audioService, IVoiceChannel voiceChannel,
            bool selfDeaf = false, bool selfMute = false)
            => audioService.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, selfDeaf, selfMute);
    }
}