namespace Lavalink4NET.Cluster.LoadBalancing.Strategies;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Cluster.Nodes;
using Microsoft.Extensions.Options;

public sealed class FilteringBalancingStrategy : INodeBalancingStrategy
{
    private readonly INodeBalancingStrategy _parentStrategy;
    private readonly IOptions<FilteringBalancingStrategyOptions> _options;

    public FilteringBalancingStrategy(INodeBalancingStrategy parentStrategy, IOptions<FilteringBalancingStrategyOptions> options)
    {
        ArgumentNullException.ThrowIfNull(parentStrategy);
        ArgumentNullException.ThrowIfNull(options);

        _parentStrategy = parentStrategy;
        _options = options;
    }

    public async ValueTask<NodeBalanceResult> ScoreAsync(ImmutableArray<ILavalinkNode> nodes, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _parentStrategy
            .ScoreAsync(nodes, cancellationToken)
            .ConfigureAwait(false);

        var filteredNodes = result.Nodes
            .Where(x => _options.Value.Filter(x))
            .ToImmutableArray();

        return new NodeBalanceResult(filteredNodes, result.Duration);
    }
}
