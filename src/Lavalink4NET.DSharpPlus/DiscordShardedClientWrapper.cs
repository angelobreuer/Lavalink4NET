/*
 *  File:   DiscordShardedClientWrapper.cs
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
    using global::DSharpPlus;
    using global::DSharpPlus.Entities;

    /// <summary>
    ///     A wrapper for the discord sharded client from the "DSharpPlus" discord client library. (https://github.com/DSharpPlus/DSharpPlus)
    /// </summary>
    public class DiscordShardedClientWrapper : DiscordClientWrapperBase, IDisposable
    {
        private readonly DiscordShardedClient _client;
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscordClientWrapper"/> class.
        /// </summary>
        /// <param name="client">the sharded discord client</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="client"/> is <see langword="null"/>.
        /// </exception>
        public DiscordShardedClientWrapper(DiscordShardedClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _client.VoiceStateUpdated += OnVoiceStateUpdated;
            _client.VoiceServerUpdated += OnVoiceServerUpdated;
        }

        /// <inheritdoc/>
        public override int ShardCount
        {
            get
            {
                EnsureNotDisposed();
                return _client.ShardClients.Count;
            }
        }

        /// <inheritdoc/>
        protected override DiscordUser CurrentUser
        {
            get
            {
                EnsureNotDisposed();
                return _client.CurrentUser;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases the unmanaged resources used by the <see
        ///     cref="DiscordShardedClientWrapper"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <see langword="true"/> to release both managed and unmanaged resources; <see
        ///     langword="false"/> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _client.VoiceStateUpdated -= OnVoiceStateUpdated;
                _client.VoiceServerUpdated -= OnVoiceServerUpdated;
            }

            _disposed = true;
        }

        /// <inheritdoc/>
        protected override DiscordClient GetClient(ulong guildId) => _client.GetShard(guildId);

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
    }
}
