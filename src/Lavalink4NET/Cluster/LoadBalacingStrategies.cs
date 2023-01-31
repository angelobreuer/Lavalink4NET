namespace Lavalink4NET.Cluster;

using System.Linq;
using Lavalink4NET.Statistics;

/// <summary>
///     Provides a set of load balancing strategies.
/// </summary>
public static class LoadBalancingStrategies
{
    /// <summary>
    ///     The load-balancing strategy that uses the node that has the least-playing player.
    /// </summary>
    public static LoadBalancingStrategy LeastPlayersStrategy { get; } = (cluster, nodes, _)
        => nodes.OrderBy(s => s.Statistics?.PlayingPlayers ?? 0).First();

    /// <summary>
    ///     The load strategy favors the node that is less used (with the lowest system load).
    /// </summary>
    public static LoadBalancingStrategy LoadStrategy { get; } = (cluster, nodes, _)
        => nodes.OrderBy(s => s.Statistics?.Processor?.SystemLoad ?? 1f).First();

    /// <summary>
    ///     The round robin load balancing strategy favors the node that has not been used the longest.
    /// </summary>
    public static LoadBalancingStrategy RoundRobinStrategy { get; } = (cluster, nodes, _)
        => nodes.OrderBy(s => s.LastUsage).First();

    /// <summary>
    ///     The score strategy favors the node that has the highest score (higher = better).
    /// </summary>
    public static LoadBalancingStrategy ScoreStrategy { get; } = (cluster, nodes, _)
        => nodes.OrderByDescending(s => CalculateScore(s.Statistics)).First();

    /// <summary>
    ///     Calculates the node score.
    /// </summary>
    /// <param name="statistics">the node statistics</param>
    /// <returns>the score for the node (higher = better)</returns>
    public static double CalculateScore(NodeStatistics? statistics)
    {
        // no statistics retrieved for the node.
        if (statistics is null || statistics.Processor is null
            || statistics.Memory is null || statistics.FrameStatistics is null)
        {
            return 0d;
        }

        // factors = the number of factors including the average calculation
        var factors = 0d;
        var totalWeight = 0d;

        void IncludeFactor(double score, double weight)
        {
            factors += weight;
            totalWeight += score * weight;
        }

        // statistics.Processor.Cores (4x) higher = better
        IncludeFactor(statistics.Processor.Cores, 4);

        // statistics.Processor.NodeLoad (3x) lower = better
        IncludeFactor(-statistics.Processor.NodeLoad, 3);

        // statistics.Processor.SystemLoad (4x) lower = better
        IncludeFactor(-statistics.Processor.SystemLoad, 4);

        // statistics.Memory.FreeMemory (2x) higher = better
        IncludeFactor(statistics.Memory.FreeMemory, 2);

        // statistics.PlayingPlayers (3x) lower = better
        IncludeFactor(-statistics.PlayingPlayers, 3);

        // statistics.FrameStatistics.AverageDeficitFrames (0.5x) lower = better
        IncludeFactor(-statistics.FrameStatistics.AverageDeficitFrames, .5d);

        // statistics.FrameStatistics.AverageNulledFrames (0.7x) lower = better (nulled frames
        // are worser than deficit)
        IncludeFactor(-statistics.FrameStatistics.AverageNulledFrames, .7d);

        // calculate average of weight
        return totalWeight / factors;
    }
}
