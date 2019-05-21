/* 
 *  File:   VoiceServerUpdateEvent.cs
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

namespace Lavalink4NET.Payloads
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    ///     The update data for the voice server update that is sent to the lavalink server when it
    ///     was received from the discord gateway.
    /// </summary>
    public sealed class VoiceServerUpdateEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VoiceServerUpdateEvent"/> class.
        /// </summary>
        /// <param name="token">the token for the voice connection</param>
        /// <param name="guildId">the id of the guild the update is for</param>
        /// <param name="endpoint">the endpoint of the voice server</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="token"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="endpoint"/> is <see langword="null"/>
        /// </exception>
        public VoiceServerUpdateEvent(string token, ulong guildId, string endpoint)
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            Token = token ?? throw new ArgumentNullException(nameof(token));
            GuildId = guildId;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="VoiceServerUpdateEvent"/> class.
        /// </summary>
        /// <param name="voiceServer">the voice server</param>
        public VoiceServerUpdateEvent(VoiceServer voiceServer)
            : this(voiceServer.Token, voiceServer.GuildId, voiceServer.Endpoint)
        {
        }

        /// <summary>
        ///     Gets the token for the voice connection.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; internal set; }

        /// <summary>
        ///     Gets the id of the guild the update is for
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong GuildId { get; internal set; }

        /// <summary>
        ///     Gets the endpoint of the voice server.
        /// </summary>
        [JsonProperty("endpoint")]
        public string Endpoint { get; internal set; }
    }
}