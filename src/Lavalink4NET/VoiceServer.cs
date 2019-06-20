/*
 *  File:   VoiceServer.cs
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

namespace Lavalink4NET
{
    using System;

    /// <summary>
    ///     Represents the information for a discord voice server.
    /// </summary>
    public sealed class VoiceServer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VoiceServer"/> class.
        /// </summary>
        /// <param name="guildId">the guild snowflake identifier the update is for</param>
        /// <param name="token">
        ///     the voice server token that is required for connecting to the voice endpoint
        /// </param>
        /// <param name="endpoint">the address of the voice server to connect to</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="token"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="endpoint"/> is <see langword="null"/>.
        /// </exception>
        public VoiceServer(ulong guildId, string token, string endpoint)
        {
            GuildId = guildId;
            Token = token ?? throw new ArgumentNullException(nameof(token));
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        /// <summary>
        ///     Gets the guild snowflake identifier the update is for.
        /// </summary>
        public ulong GuildId { get; }

        /// <summary>
        ///     Gets the voice server token that is required for connecting to the voice endpoint.
        /// </summary>
        public string Token { get; }

        /// <summary>
        ///     Gets the address of the voice server to connect to.
        /// </summary>
        public string Endpoint { get; }
    }
}