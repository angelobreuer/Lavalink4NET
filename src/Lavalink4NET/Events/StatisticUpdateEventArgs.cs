/*
 *  File:   StatisticUpdateEventArgs.cs
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

namespace Lavalink4NET.Events
{
    using System;
    using Statistics;

    /// <summary>
    ///     The event arguments for the <see cref="LavalinkNode.StatisticsUpdated"/> event.
    /// </summary>
    public sealed class StatisticUpdateEventArgs
        : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StatisticUpdateEventArgs"/> class.
        /// </summary>
        /// <param name="players">the number of players the node is holding</param>
        /// <param name="playingPlayers">
        ///     the number of players that are currently playing using the node
        /// </param>
        /// <param name="uptime">the uptime from the node (how long the node is online)</param>
        /// <param name="memory">the usage statistics for the memory of the node</param>
        /// <param name="processor">the usage statistics for the processor of the node</param>
        /// <param name="frameStatistics">the frame statistics of the node</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="memory"/> parameter is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="processor"/> parameter is <see langword="null"/>.
        /// </exception>
        public StatisticUpdateEventArgs(int players, int playingPlayers, TimeSpan uptime, MemoryStatistics memory, ProcessorStatistics processor, FrameStatistics frameStatistics)
        {
            Players = players;
            PlayingPlayers = playingPlayers;
            Uptime = uptime;
            FrameStatistics = frameStatistics;
            Memory = memory ?? throw new ArgumentNullException(nameof(memory));
            Processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        /// <summary>
        ///     Gets the number of players the node is holding.
        /// </summary>
        public int Players { get; internal set; }

        /// <summary>
        ///     Gets the number of players that are currently playing using the node.
        /// </summary>
        public int PlayingPlayers { get; internal set; }

        /// <summary>
        ///     Gets the uptime from the node (how long the node is online).
        /// </summary>
        public TimeSpan Uptime { get; internal set; }

        /// <summary>
        ///     Gets the usage statistics for the memory of the node.
        /// </summary>
        public MemoryStatistics Memory { get; internal set; }

        /// <summary>
        ///     Gets the usage statistics for the processor of the node.
        /// </summary>
        public ProcessorStatistics Processor { get; internal set; }

        /// <summary>
        ///     Gets the frame statistics of the node.
        /// </summary>
        public FrameStatistics FrameStatistics { get; internal set; }
    }
}