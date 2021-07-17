/*
 *  File:   VoiceStateUpdatePayload.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2021
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
    using Newtonsoft.Json;

    internal sealed class VoiceStateUpdatePayload
    {
        public VoiceStateUpdatePayload(ulong guildId, ulong? channelId, bool isSelfMuted = false, bool isSelfDeafened = false)
        {
            GuildId = guildId;
            ChannelId = channelId;
            IsSelfMuted = isSelfMuted;
            IsSelfDeafened = isSelfDeafened;
        }

        [JsonRequired, JsonProperty("guild_id")]
        public ulong GuildId { get; }

        [JsonProperty("channel_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? ChannelId { get; }

        [JsonRequired, JsonProperty("self_mute")]
        public bool IsSelfMuted { get; }

        [JsonRequired, JsonProperty("self_deaf")]
        public bool IsSelfDeafened { get; }
    }
}