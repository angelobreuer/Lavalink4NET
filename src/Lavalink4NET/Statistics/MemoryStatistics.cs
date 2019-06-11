/*
 *  File:   MemoryStatistics.cs
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

namespace Lavalink4NET.Statistics
{
    using Newtonsoft.Json;

    /// <summary>
    ///     A wrapper for the statistics.memory object in the statistics update from the lavalink server.
    /// </summary>
    public sealed class MemoryStatistics
    {
        /// <summary>
        ///     The free RAM memory in bytes.
        /// </summary>
        [JsonRequired, JsonProperty("free")]
        public ulong FreeMemory { get; internal set; }

        /// <summary>
        ///     The used RAM memory in bytes.
        /// </summary>
        [JsonRequired, JsonProperty("used")]
        public ulong UsedMemory { get; internal set; }

        /// <summary>
        ///     The allocated RAM memory in bytes.
        /// </summary>
        [JsonRequired, JsonProperty("allocated")]
        public ulong AllocatedMemory { get; internal set; }

        /// <summary>
        ///     The reservable RAM memory in bytes.
        /// </summary>
        [JsonRequired, JsonProperty("reservable")]
        public ulong ReservableMemory { get; internal set; }
    }
}