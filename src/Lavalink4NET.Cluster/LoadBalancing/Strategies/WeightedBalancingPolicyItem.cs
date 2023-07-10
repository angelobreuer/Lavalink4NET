namespace Lavalink4NET.Cluster.LoadBalancing.Strategies;

public readonly record struct WeightedBalancingPolicyItem(INodeBalancingStrategy Policy, double Weight);