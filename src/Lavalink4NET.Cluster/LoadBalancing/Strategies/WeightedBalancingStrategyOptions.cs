namespace Lavalink4NET.Cluster.LoadBalancing.Strategies;

using System.Collections.Immutable;

public sealed record class WeightedBalancingStrategyOptions
{
    public ImmutableArray<WeightedBalancingPolicyItem> Policies { get; set; } = ImmutableArray<WeightedBalancingPolicyItem>.Empty;
}