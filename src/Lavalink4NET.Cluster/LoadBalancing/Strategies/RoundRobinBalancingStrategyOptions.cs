namespace Lavalink4NET.Cluster.LoadBalancing.Strategies;

using System;

public sealed record class RoundRobinBalancingStrategyOptions
{
    public TimeSpan Duration { get; set; } = default;
}
