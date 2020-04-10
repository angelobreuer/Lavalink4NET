/*
 *  File:   IAudioService.cs
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

namespace Lavalink4NET
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Player;
    using Rest;

    /// <summary>
    ///     The interface for a lavalink audio provider service.
    /// </summary>
    public interface IAudioService : IDisposable, ILavalinkRestClient
    {
        /// <summary>
        ///     Gets the audio player for the specified <paramref name="guildId"/>.
        /// </summary>
        /// <typeparam name="TPlayer">the type of the player to use</typeparam>
        /// <param name="guildId">the guild identifier to get the player for</param>
        /// <returns>the player for the guild</returns>
        TPlayer GetPlayer<TPlayer>(ulong guildId) where TPlayer : LavalinkPlayer;

        /// <summary>
        ///     Gets the audio player for the specified <paramref name="guildId"/>.
        /// </summary>
        /// <param name="guildId">the guild identifier to get the player for</param>
        /// <returns>the player for the guild</returns>
        LavalinkPlayer GetPlayer(ulong guildId);

        /// <summary>
        ///     Gets all players of the specified <typeparamref name="TPlayer"/>.
        /// </summary>
        /// <typeparam name="TPlayer">
        ///     the type of the players to get; use <see cref="LavalinkPlayer"/> to get all players
        /// </typeparam>
        /// <returns>the player list</returns>
        IReadOnlyList<TPlayer> GetPlayers<TPlayer>() where TPlayer : LavalinkPlayer;

        /// <summary>
        ///     Gets a value indicating whether a player is created for the specified <paramref name="guildId"/>.
        /// </summary>
        /// <param name="guildId">
        ///     the snowflake identifier of the guild to create the player for
        /// </param>
        /// <returns>
        ///     a value indicating whether a player is created for the specified <paramref name="guildId"/>
        /// </returns>
        bool HasPlayer(ulong guildId);

        /// <summary>
        ///     Initializes the audio service asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        Task InitializeAsync();

        /// <summary>
        ///     Joins the channel specified by <paramref name="voiceChannelId"/> asynchronously.
        /// </summary>
        /// <typeparam name="TPlayer">the type of the player to create</typeparam>
        /// <param name="guildId">the guild snowflake identifier</param>
        /// <param name="voiceChannelId">the snowflake identifier of the voice channel to join</param>
        /// <param name="selfDeaf">a value indicating whether the bot user should be self deafened</param>
        /// <param name="selfMute">a value indicating whether the bot user should be self muted</param>
        /// <returns>
        ///     a task that represents the asynchronous operation
        ///     <para>the audio player</para>
        /// </returns>
        Task<TPlayer> JoinAsync<TPlayer>(ulong guildId, ulong voiceChannelId, bool selfDeaf = false, bool selfMute = false) where TPlayer : LavalinkPlayer, new();

        /// <summary>
        ///     Joins the channel specified by <paramref name="voiceChannelId"/> asynchronously.
        /// </summary>
        /// <typeparam name="TPlayer">the type of the player to create</typeparam>
        /// <param name="playerFactory">the factory used to create the player instance</param>
        /// <param name="guildId">the guild snowflake identifier</param>
        /// <param name="voiceChannelId">the snowflake identifier of the voice channel to join</param>
        /// <param name="selfDeaf">a value indicating whether the bot user should be self deafened</param>
        /// <param name="selfMute">a value indicating whether the bot user should be self muted</param>
        /// <returns>
        ///     a task that represents the asynchronous operation
        ///     <para>the audio player</para>
        /// </returns>
        Task<TPlayer> JoinAsync<TPlayer>(PlayerFactory<TPlayer> playerFactory, ulong guildId, ulong voiceChannelId, bool selfDeaf = false, bool selfMute = false) where TPlayer : LavalinkPlayer;

        /// <summary>
        ///     Joins the channel specified by <paramref name="voiceChannelId"/> asynchronously.
        /// </summary>
        /// <param name="guildId">the guild snowflake identifier</param>
        /// <param name="voiceChannelId">the snowflake identifier of the voice channel to join</param>
        /// <param name="selfDeaf">a value indicating whether the bot user should be self deafened</param>
        /// <param name="selfMute">a value indicating whether the bot user should be self muted</param>
        /// <returns>
        ///     a task that represents the asynchronous operation
        ///     <para>the audio player</para>
        /// </returns>
        Task<LavalinkPlayer> JoinAsync(ulong guildId, ulong voiceChannelId, bool selfDeaf = false, bool selfMute = false);
    }
}