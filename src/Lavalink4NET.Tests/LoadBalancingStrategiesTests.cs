/*
 *  File:   LoadBalancingStrategiesTests.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2022
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

namespace Lavalink4NET.Tests
{
    using System;
    using Lavalink4NET.Cluster;
    using Lavalink4NET.Statistics;
    using Xunit;

    /// <summary>
    ///     Contains tests for load balancing strategies ( <see cref="LoadBalancingStrategies"/>).
    /// </summary>
    public sealed class LoadBalancingStrategiesTests
    {
        /// <summary>
        ///     Tests the score balancing strategy calculation method (
        ///     <see cref="LoadBalancingStrategies.CalculateScore(StatisticUpdateEventArgs)"/>) with
        ///     good and bad statistics.
        /// </summary>
        [Fact]
        public void TestScoreBalancingStrategy()
        {
            var badNode = new NodeStatistics(10, 200, TimeSpan.FromHours(60),
                new MemoryStatistics { AllocatedMemory = 10000, FreeMemory = 1000, ReservableMemory = 1000, UsedMemory = 5000 },
                new ProcessorStatistics { Cores = 10, NodeLoad = 100, SystemLoad = 100 },
                new FrameStatistics { AverageDeficitFrames = 10, AverageFramesSent = 100000, AverageNulledFrames = 100 });

            var goodNode = new NodeStatistics(1, 1, TimeSpan.FromSeconds(100),
                new MemoryStatistics { AllocatedMemory = 1000, FreeMemory = 500, ReservableMemory = 100, UsedMemory = 1000 },
                new ProcessorStatistics { Cores = 54, NodeLoad = 20, SystemLoad = 30 },
                new FrameStatistics { AverageDeficitFrames = 0, AverageFramesSent = 10000, AverageNulledFrames = 0 });

            var badScore = LoadBalancingStrategies.CalculateScore(badNode);
            var goodScore = LoadBalancingStrategies.CalculateScore(goodNode);

            Assert.True(badScore < goodScore, $"bad score ({badScore}) < good score ({goodScore})");
        }
    }
}
