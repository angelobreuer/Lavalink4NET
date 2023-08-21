namespace Lavalink4NET.Cluster.LoadBalancing.Strategies;

using System.Collections.Immutable;
using Lavalink4NET.Cluster.Nodes;

public interface INodeBalancingStrategy
{
    ValueTask<NodeBalanceResult> ScoreAsync(ImmutableArray<ILavalinkNode> nodes, CancellationToken cancellationToken = default);
}
