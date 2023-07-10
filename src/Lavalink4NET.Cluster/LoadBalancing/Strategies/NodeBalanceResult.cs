namespace Lavalink4NET.Cluster.LoadBalancing.Strategies;

using System.Collections.Immutable;

public readonly record struct NodeBalanceResult(ImmutableArray<ScoredLavalinkNode> Nodes, TimeSpan Duration = default);
