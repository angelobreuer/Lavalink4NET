/*
 *  File:   DiscordClientWrapper.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2019
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

namespace Lavalink4NET.DSharpPlus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::DSharpPlus;
    using global::DSharpPlus.EventArgs;
    using Lavalink4NET.Events;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///     A wrapper for the discord client from the "DSharpPlus" discord client library. (https://github.com/DSharpPlus/DSharpPlus)
    /// </summary>
    public sealed class DiscordClientWrapper : IDiscordClientWrapper, IDisposable
    {
        private readonly DiscordClient _client;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscordClientWrapper"/> class.
        /// </summary>
        /// <param name="client">the sharded discord client</param>
        public DiscordClientWrapper(DiscordClient client)
        {
            _client = client;

            _client.VoiceStateUpdated += OnVoiceStateUpdated;
            _client.VoiceServerUpdated += OnVoiceServerUpdated;
        }

        /// <summary>
        ///     An asynchronous event which is triggered when the voice server was updated.
        /// </summary>
        public event Events.AsyncEventHandler<VoiceServer> VoiceServerUpdated;

        /// <summary>
        ///     An asynchronous event which is triggered when a user voice state was updated.
        /// </summary>
        public event Events.AsyncEventHandler<Events.VoiceStateUpdateEventArgs> VoiceStateUpdated;

        /// <summary>
        ///     Gets the current user snowflake identifier value.
        /// </summary>
        public ulong CurrentUserId => _client.CurrentUser.Id;

        /// <summary>
        ///     Gets the number of total shards the bot uses.
        /// </summary>
        public int ShardCount => _client.ShardCount;

        /// <summary>
        ///     Disposes the wrapper and unregisters all events attached to the discord client.
        /// </summary>
        public void Dispose()
        {
            _client.VoiceStateUpdated -= OnVoiceStateUpdated;
            _client.VoiceServerUpdated -= OnVoiceServerUpdated;
        }

        /// <summary>
        ///     Gets the snowflake identifier values of the users in the voice channel specified by
        ///     <paramref name="voiceChannelId"/> (the snowflake identifier of the voice channel).
        /// </summary>
        /// <param name="guildId">the guild identifier snowflake where the channel is in</param>
        /// <param name="voiceChannelId">the snowflake identifier of the voice channel</param>
        /// <returns>
        ///     a task that represents the asynchronous operation
        ///     <para>the snowflake identifier values of the users in the voice channel</para>
        /// </returns>
        public async Task<IEnumerable<ulong>> GetChannelUsersAsync(ulong guildId, ulong voiceChannelId)
        {
            var guild = await _client.GetGuildAsync(guildId)
                ?? throw new ArgumentException("Invalid or inaccessible guild: " + guildId, nameof(guildId));

            var channel = guild.GetChannel(voiceChannelId)
                ?? throw new ArgumentException("Invalid or inaccessible voice channel: " + voiceChannelId, nameof(voiceChannelId));

            return channel.Users.Select(s => s.Id);
        }

        /// <summary>
        ///     Awaits the initialization of the discord client asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        public async Task InitializeAsync()
        {
            var startTime = DateTimeOffset.UtcNow;

            // await until current user arrived
            while (_client.CurrentUser is null)
            {
                await Task.Delay(10);

                // timeout exceeded
                if (DateTimeOffset.UtcNow - startTime > TimeSpan.FromSeconds(10))
                {
                    throw new TimeoutException("Waited 10 seconds for current user to arrive! Make sure you start " +
                        "the discord client, before initializing the discord wrapper!");
                }
            }
        }

        /// <summary>
        ///     Sends a voice channel state update asynchronously.
        /// </summary>
        /// <param name="guildId">the guild snowflake identifier</param>
        /// <param name="voiceChannelId">
        ///     the snowflake identifier of the voice channel to join (if <see langword="null"/> the
        ///     client should disconnect from the voice channel).
        /// </param>
        /// <param name="selfDeaf">a value indicating whether the bot user should be self deafened</param>
        /// <param name="selfMute">a value indicating whether the bot user should be self muted</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        public Task SendVoiceUpdateAsync(ulong guildId, ulong? voiceChannelId, bool selfDeaf = false, bool selfMute = false)
        {
            var payload = new JObject();
            var data = new VoiceStateUpdatePayload(guildId, voiceChannelId, selfMute, selfDeaf);

            payload.Add("op", 4);
            payload.Add("d", JObject.FromObject(data));

            var message = JsonConvert.SerializeObject(payload, Formatting.None);
            _client.GetWebSocketClient().SendMessage(message);
            return Task.CompletedTask;
        }

        /// <summary>
        ///     The asynchronous callback when a voice server update was received.
        /// </summary>
        /// <param name="voiceServer">the voice server data</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        private Task OnVoiceServerUpdated(VoiceServerUpdateEventArgs voiceServer)
        {
            var args = new VoiceServer(voiceServer.Guild.Id, voiceServer.GetVoiceToken(), voiceServer.Endpoint);
            return VoiceServerUpdated.InvokeAsync(this, args);
        }

        /// <summary>
        ///     The asynchronous callback when a voice state update was received.
        /// </summary>
        /// <param name="voiceServer">the voice state data</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        private Task OnVoiceStateUpdated(global::DSharpPlus.EventArgs.VoiceStateUpdateEventArgs eventArgs)
        {
            var sessionId = eventArgs.GetSessionId();
            var guildId = eventArgs.Before?.Channel?.Guild?.Id ?? eventArgs.After.Channel.Guild.Id;
            var oldVoiceState = new VoiceState(eventArgs.Before?.Channel?.Id, guildId, sessionId);
            var voiceState = new VoiceState(eventArgs.After.Channel?.Id, guildId, sessionId);
            var args = new Events.VoiceStateUpdateEventArgs(eventArgs.User.Id, voiceState, oldVoiceState);
            return VoiceStateUpdated.InvokeAsync(this, args);
        }
    }
}