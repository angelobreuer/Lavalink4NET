/*
 *  File:   FrameStatistics.cs
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

namespace Lavalink4NET.Statistics
{
    using Newtonsoft.Json;

    /// <summary>
    ///     The frame statistics of a lavalink node.
    /// </summary>
    public sealed class FrameStatistics
    {
        /// <summary>
        ///     Gets the number of average frames sent per minute.
        /// </summary>
        [JsonRequired, JsonProperty("sent")]
        public int AverageFramesSent { get; internal set; }

        /// <summary>
        ///     Gets the number of average nulled frames per minute.
        /// </summary>
        [JsonRequired, JsonProperty("nulled")]
        public int AverageNulledFrames { get; internal set; }

        /// <summary>
        ///     Gets the number of average deficit frames per minute.
        /// </summary>
        [JsonRequired, JsonProperty("deficit")]
        public int AverageDeficitFrames { get; internal set; }
    }
}