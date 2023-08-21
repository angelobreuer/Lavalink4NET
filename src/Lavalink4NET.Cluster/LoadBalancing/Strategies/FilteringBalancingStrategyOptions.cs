namespace Lavalink4NET.Cluster.LoadBalancing.Strategies;

using System;

public sealed record class FilteringBalancingStrategyOptions
{
    public TimeSpan Duration { get; init; } = default;

    public Func<ScoredLavalinkNode, bool> Filter { get; set; } = static _ => true;
}
