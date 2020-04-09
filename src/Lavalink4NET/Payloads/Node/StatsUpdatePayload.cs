/*
 *  File:   StatsUpdatePayload.cs
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
    using Newtonsoft.Json;
    using Statistics;

    /// <summary>
    ///     The strongly-typed representation of a node statistics payload received from the
    ///     lavalink node (in serialized JSON format). For more reference see https://github.com/Frederikam/Lavalink/blob/master/IMPLEMENTATION.md
    /// </summary>
    public sealed class StatsUpdatePayload : IPayload
    {
        /// <summary>
        ///     Gets the operation code for the payload.
        /// </summary>
        [JsonRequired, JsonProperty("op")]
        public OpCode OpCode => OpCode.NodeStats;

        /// <summary>
        ///     Gets the number of players the node is holding.
        /// </summary>
        [JsonRequired, JsonProperty("players")]
        public int Players { get; internal set; }

        /// <summary>
        ///     Gets the number of players that are currently playing using the node.
        /// </summary>
        [JsonRequired, JsonProperty("playingPlayers")]
        public int PlayingPlayers { get; internal set; }

        [JsonRequired, JsonProperty("uptime")]
        internal long RawUptime { set => Uptime = TimeSpan.FromMilliseconds(value); }

        /// <summary>
        ///     Gets the uptime from the node (how long the node is online).
        /// </summary>
        [JsonIgnore]
        public TimeSpan Uptime { get; internal set; }

        /// <summary>
        ///     Gets usage statistics for the memory of the node.
        /// </summary>
        [JsonRequired, JsonProperty("memory")]
        public MemoryStatistics Memory { get; internal set; }

        /// <summary>
        ///     Gets usage statistics for the processor of the node.
        /// </summary>
        [JsonRequired, JsonProperty("cpu")]
        public ProcessorStatistics Processor { get; internal set; }

        /// <summary>
        ///     Gets frame statistics of the node.
        /// </summary>
        [JsonProperty("frameStats")]
        public FrameStatistics FrameStatistics { get; internal set; }
    }
}