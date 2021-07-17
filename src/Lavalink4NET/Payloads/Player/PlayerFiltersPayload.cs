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
    using System.Linq;
    using Lavalink4NET.Filters;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///     The strongly-typed representation of a player filters update payload sent to the
    ///     lavalink node (in serialized JSON format). For more reference see the lavalink client
    ///     implementation documentation https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md
    /// </summary>
    public sealed class PlayerFiltersPayload : IPlayerPayload
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerFiltersPayload"/> class.
        /// </summary>
        /// <param name="guildId">
        ///     the guild snowflake identifier the player equalizer update is for
        /// </param>
        /// <param name="filters">the player filters</param>
        public PlayerFiltersPayload(ulong guildId, IReadOnlyDictionary<string, IFilterOptions> filters)
        {
            GuildId = guildId.ToString();
            Filters = filters.ToDictionary(x => x.Key, x => JToken.FromObject(x.Value));
        }

        /// <summary>
        ///     Gets the operation code for the payload.
        /// </summary>
        [JsonRequired, JsonProperty("op")]
        public OpCode OpCode => OpCode.PlayerFilters;

        /// <summary>
        ///     Gets the guild snowflake identifier the player equalizer update is for.
        /// </summary>
        [JsonRequired, JsonProperty("guildId")]
        public string GuildId { get; internal set; }

        [JsonRequired, JsonExtensionData, JsonProperty("filters")]
        internal IDictionary<string, JToken> Filters { get; set; }
    }
}
