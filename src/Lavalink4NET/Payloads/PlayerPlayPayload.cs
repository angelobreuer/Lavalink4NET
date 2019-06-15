/*
 *  File:   PlayerPlayPayload.cs
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
    ///     The strongly-typed representation of a player play payload sent to the lavalink node (in
    ///     serialized JSON format). For more reference see https://github.com/Frederikam/Lavalink/blob/master/IMPLEMENTATION.md
    /// </summary>
    public sealed class PlayerPlayPayload
        : IPayload, IPlayerPayload
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerPlayPayload"/> class.
        /// </summary>
        /// <param name="guildId">the guild snowflake identifier the voice update is for</param>
        /// <param name="trackIdentifier">the track identifier that the player should play</param>
        /// <param name="startTime">the track start position</param>
        /// <param name="endTime">the track end position</param>
        /// <param name="noReplace">
        ///     a value indicating whether the track play should be ignored if the same track is
        ///     currently playing
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="trackIdentifier"/> is <see langword="null"/>
        /// </exception>
        public PlayerPlayPayload(ulong guildId, string trackIdentifier, TimeSpan? startTime = null, TimeSpan? endTime = null, bool noReplace = false)
        {
            GuildId = guildId.ToString();
            TrackIdentifier = trackIdentifier ?? throw new ArgumentNullException(nameof(trackIdentifier));
            StartTime = (int?)startTime?.TotalMilliseconds;
            EndTime = (int?)endTime?.TotalMilliseconds;
            NoReplace = noReplace;
        }

        /// <summary>
        ///     Gets the operation code for the payload.
        /// </summary>
        [JsonRequired, JsonProperty("op")]
        public OpCode OpCode => OpCode.PlayerPlay;

        /// <summary>
        ///     Gets the guild snowflake identifier the player update is for.
        /// </summary>
        [JsonRequired, JsonProperty("guildId")]
        public string GuildId { get; internal set; }

        /// <summary>
        ///     Gets the track identifier that the player should play.
        /// </summary>
        [JsonRequired, JsonProperty("track")]
        public string TrackIdentifier { get; internal set; }

        /// <summary>
        ///     Gets the track start position in milliseconds.
        /// </summary>
        [JsonRequired]
        [JsonProperty("startTime", NullValueHandling = NullValueHandling.Ignore)]
        public int? StartTime { get; internal set; }

        /// <summary>
        ///     Gets the track end position in milliseconds.
        /// </summary>
        [JsonRequired]
        [JsonProperty("endTime", NullValueHandling = NullValueHandling.Ignore)]
        public int? EndTime { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether the track play should be ignored if the same track is
        ///     currently playing.
        /// </summary>
        [JsonRequired, JsonProperty("noReplace")]
        public bool NoReplace { get; internal set; }
    }
}