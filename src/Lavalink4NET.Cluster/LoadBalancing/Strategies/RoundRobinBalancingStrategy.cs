namespace Lavalink4NET.Cluster.LoadBalancing.Strategies;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Cluster.Nodes;
using Microsoft.Extensions.Options;

public sealed class RoundRobinBalancingStrategy : INodeBalancingStrategy
{
    private readonly RoundRobinBalancingStrategyOptions _options;
    private uint _index;

    public RoundRobinBalancingStrategy(IOptions<RoundRobinBalancingStrategyOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Value;
    }

    public ValueTask<NodeBalanceResult> ScoreAsync(ImmutableArray<ILavalinkNode> nodes, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var selectedNodeIndex = (Interlocked.Increment(ref _index) - 1) % nodes.Length;
        var builder = ImmutableArray.CreateBuilder<ScoredLavalinkNode>(nodes.Length);
        var step = 1.0D / nodes.Length;

        for (var index = 0; index < nodes.Length; index++)
        {
            // distance between selected node and current node
            var distance = index - selectedNodeIndex;

            if (distance < 0)
            {
                distance += nodes.Length;
            }

            var score = step * (nodes.Length - distance);
            builder.Add(new ScoredLavalinkNode(nodes[index], score));
        }

        var result = new NodeBalanceResult(builder.MoveToImmutable(), _options.Duration);
        return new ValueTask<NodeBalanceResult>(result);
    }
}