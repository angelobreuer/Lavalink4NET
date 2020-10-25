/*
 *  File:   PlayerEqualizerPayload.cs
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

namespace Lavalink4NET.Payloads.Player
{
    using System.Collections.Generic;
    using Lavalink4NET.Player;
    using Newtonsoft.Json;

    /// <summary>
    ///     The strongly-typed representation of a player equalizer update payload sent to the
    ///     lavalink node (in serialized JSON format). For more reference see the lavalink client
    ///     implementation documentation https://github.com/Frederikam/Lavalink/blob/master/IMPLEMENTATION.md
    /// </summary>
    public sealed class PlayerEqualizerPayload : IPlayerPayload
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerEqualizerPayload"/> class.
        /// </summary>
        /// <param name="guildId">
        ///     the guild snowflake identifier the player equalizer update is for
        /// </param>
        /// <param name="bands">the equalizer bands</param>
        public PlayerEqualizerPayload(ulong guildId, IReadOnlyList<EqualizerBand> bands)
        {
            GuildId = guildId.ToString();
            Bands = bands;
        }

        /// <summary>
        ///     Gets the operation code for the payload.
        /// </summary>
        [JsonRequired, JsonProperty("op")]
        public OpCode OpCode => OpCode.PlayerEqualizer;

        /// <summary>
        ///     Gets the guild snowflake identifier the player equalizer update is for.
        /// </summary>
        [JsonRequired, JsonProperty("guildId")]
        public string GuildId { get; internal set; }

        /// <summary>
        ///     Gets the equalizer bands.
        /// </summary>
        [JsonRequired, JsonProperty("bands")]
        public IReadOnlyList<EqualizerBand> Bands { get; internal set; }
    }
}
