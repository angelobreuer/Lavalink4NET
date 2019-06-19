namespace Lavalink4NET.Tests
{
    using System;
    using Lavalink4NET.Cluster;
    using Lavalink4NET.Events;
    using Lavalink4NET.Statistics;
    using Xunit;

    /// <summary>
    ///     Contains tests for load balancing strategies ( <see cref="LoadBalacingStrategies"/>).
    /// </summary>
    public sealed class LoadBalancingStrategiesTests
    {
        /// <summary>
        ///     Tests the score balancing strategy calculation method (
        ///     <see cref="LoadBalacingStrategies.CalculateScore(StatisticUpdateEventArgs)"/>) with
        ///     good and bad statistics.
        /// </summary>
        [Fact]
        public void TestScoreBalancingStrategy()
        {
            var badNode = new StatisticUpdateEventArgs(10, 200, TimeSpan.FromHours(60),
                new MemoryStatistics { AllocatedMemory = 10000, FreeMemory = 1000, ReservableMemory = 1000, UsedMemory = 5000 },
                new ProcessorStatistics { Cores = 10, NodeLoad = 100, SystemLoad = 100 },
                new FrameStatistics { AverageDeficitFrames = 10, AverageFramesSent = 100000, AverageNulledFrames = 100 });

            var goodNode = new StatisticUpdateEventArgs(1, 1, TimeSpan.FromSeconds(100),
                new MemoryStatistics { AllocatedMemory = 1000, FreeMemory = 500, ReservableMemory = 100, UsedMemory = 1000 },
                new ProcessorStatistics { Cores = 54, NodeLoad = 20, SystemLoad = 30 },
                new FrameStatistics { AverageDeficitFrames = 0, AverageFramesSent = 10000, AverageNulledFrames = 0 });

            var badScore = LoadBalacingStrategies.CalculateScore(badNode);
            var goodScore = LoadBalacingStrategies.CalculateScore(goodNode);

            Assert.True(badScore < goodScore, $"bad score ({badScore}) < good score ({goodScore})");
        }
    }
}