/*
 *  File:   VoiceUpdatePayload.cs
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

namespace Lavalink4NET.Payloads.Node
{
    using System;
    using Lavalink4NET.Payloads.Events;
    using Newtonsoft.Json;

    /// <summary>
    ///     The representation of a voice update lavalink payload.
    /// </summary>
    public sealed class VoiceUpdatePayload : IPayload, IPlayerPayload
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VoiceUpdatePayload"/> class.
        /// </summary>
        /// <param name="guildId">the guild snowflake identifier the voice update is for</param>
        /// <param name="sessionId">
        ///     the discord voice state session identifier received from the voice state update payload
        /// </param>
        /// <param name="voiceServerUpdateEvent">the voice server update event</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="sessionId"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="voiceServerUpdateEvent"/> is <see langword="null"/>
        /// </exception>
        public VoiceUpdatePayload(ulong guildId, string sessionId, VoiceServerUpdateEvent voiceServerUpdateEvent)
        {
            GuildId = guildId.ToString();
            SessionId = sessionId ?? throw new ArgumentNullException(nameof(sessionId));
            VoiceServerUpdateEvent = voiceServerUpdateEvent ?? throw new ArgumentNullException(nameof(voiceServerUpdateEvent));
        }

        /// <summary>
        ///     Gets the operation code for the payload.
        /// </summary>
        [JsonRequired, JsonProperty("op")]
        public OpCode OpCode => OpCode.GuildVoiceUpdate;

        /// <summary>
        ///     Gets the guild snowflake identifier the voice update is for.
        /// </summary>
        [JsonRequired, JsonProperty("guildId")]
        public string GuildId { get; internal set; }

        /// <summary>
        ///     Gets the discord voice state session identifier received from the voice state update payload.
        /// </summary>
        [JsonRequired, JsonProperty("sessionId")]
        public string SessionId { get; internal set; }

        /// <summary>
        ///     Gets the voice server update event.
        /// </summary>
        [JsonRequired, JsonProperty("event")]
        public VoiceServerUpdateEvent VoiceServerUpdateEvent { get; internal set; }
    }
}