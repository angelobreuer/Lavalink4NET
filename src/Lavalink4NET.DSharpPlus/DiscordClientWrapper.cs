/*
 *  File:   DiscordClientWrapper.cs
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
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscordClientWrapper"/> class.
        /// </summary>
        /// <param name="client">the sharded discord client</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="client"/> is <see langword="null"/>.
        /// </exception>
        public DiscordClientWrapper(DiscordClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _client.VoiceStateUpdated += OnVoiceStateUpdated;
            _client.VoiceServerUpdated += OnVoiceServerUpdated;
        }

        /// <inheritdoc/>
        public event Events.AsyncEventHandler<VoiceServer> VoiceServerUpdated;

        /// <inheritdoc/>
        public event Events.AsyncEventHandler<Events.VoiceStateUpdateEventArgs> VoiceStateUpdated;

        /// <inheritdoc/>
        public ulong CurrentUserId
        {
            get
            {
                EnsureNotDisposed();
                return _client.CurrentUser.Id;
            }
        }

        /// <inheritdoc/>
        public int ShardCount
        {
            get
            {
                EnsureNotDisposed();
                return _client.ShardCount;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            _client.VoiceStateUpdated -= OnVoiceStateUpdated;
            _client.VoiceServerUpdated -= OnVoiceServerUpdated;
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
        public async Task<IEnumerable<ulong>> GetChannelUsersAsync(ulong guildId, ulong voiceChannelId)
        {
            EnsureNotDisposed();

            var guild = await _client.GetGuildAsync(guildId)
                ?? throw new ArgumentException("Invalid or inaccessible guild: " + guildId, nameof(guildId));

            var channel = guild.GetChannel(voiceChannelId)
                ?? throw new ArgumentException("Invalid or inaccessible voice channel: " + voiceChannelId, nameof(voiceChannelId));

            return channel.Users.Select(s => s.Id);
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
        public async Task InitializeAsync()
        {
            EnsureNotDisposed();

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

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
        public async Task SendVoiceUpdateAsync(ulong guildId, ulong? voiceChannelId, bool selfDeaf = false, bool selfMute = false)
        {
            EnsureNotDisposed();

            var payload = new JObject();
            var data = new VoiceStateUpdatePayload(guildId, voiceChannelId, selfMute, selfDeaf);

            payload.Add("op", 4);
            payload.Add("d", JObject.FromObject(data));

            var message = JsonConvert.SerializeObject(payload, Formatting.None);
            await _client.GetWebSocketClient().SendMessageAsync(message);
        }

        /// <summary>
        ///     Throws an <see cref="ObjectDisposedException"/> if the <see
        ///     cref="DiscordClientWrapper"/> is disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
        private void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(DiscordClientWrapper));
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
        private Task OnVoiceServerUpdated(VoiceServerUpdateEventArgs voiceServer)
        {
            EnsureNotDisposed();

            var args = new VoiceServer(voiceServer.Guild.Id, voiceServer.GetVoiceToken(), voiceServer.Endpoint);
            return VoiceServerUpdated.InvokeAsync(this, args);
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
        private Task OnVoiceStateUpdated(global::DSharpPlus.EventArgs.VoiceStateUpdateEventArgs eventArgs)
        {
            EnsureNotDisposed();

            // create voice states
            var oldVoiceState = eventArgs.Before?.Channel is null ? null : new VoiceState(
                eventArgs.Before.Channel.Id, eventArgs.Before.Guild.Id, eventArgs.Before.GetSessionId());

            var voiceState = eventArgs.After?.Channel is null ? null : new VoiceState(
                eventArgs.After.Channel.Id, eventArgs.After.Guild.Id, eventArgs.After.GetSessionId());

            // invoke event
            return VoiceStateUpdated.InvokeAsync(this,
                new Events.VoiceStateUpdateEventArgs(eventArgs.User.Id, voiceState, oldVoiceState));
        }
    }
}