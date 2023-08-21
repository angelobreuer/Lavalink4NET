namespace Lavalink4NET.Cluster.LoadBalancing.Strategies;

using Lavalink4NET.Cluster.Nodes;

public readonly record struct ScoredLavalinkNode(ILavalinkNode Node, double Score);
