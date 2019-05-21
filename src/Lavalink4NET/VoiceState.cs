/* 
 *  File:   VoiceState.cs
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
    /// <summary>
    ///     Represents the information for a discord user voice state.
    /// </summary>
    public sealed class VoiceState
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VoiceState"/> class.
        /// </summary>
        /// <param name="voiceChannelId">the voice channel identifier</param>
        /// <param name="guildId">
        ///     the guild snowflake identifier the voice state update is for
        /// </param>
        /// <param name="voiceSessionId">the voice session identifier</param>
        public VoiceState(ulong? voiceChannelId, ulong guildId, string voiceSessionId)
        {
            GuildId = guildId;
            VoiceChannelId = voiceChannelId;
            VoiceSessionId = voiceSessionId;
        }

        /// <summary>
        ///     Gets the voice channel identifier.
        /// </summary>
        public ulong? VoiceChannelId { get; }

        /// <summary>
        ///     Gets the guild snowflake identifier the voice state update is for.
        /// </summary>
        public ulong GuildId { get; }

        /// <summary>
        ///     Gets the voice session identifier.
        /// </summary>
        public string VoiceSessionId { get; }
    }
}