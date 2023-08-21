namespace Lavalink4NET.Cluster.LoadBalancing.Strategies;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Cluster.Nodes;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;

public sealed class WeightedBalancingStrategy : INodeBalancingStrategy
{
    private readonly ImmutableArray<WeightedNodeBalancingStrategyItemHandle> _handles;

    public WeightedBalancingStrategy(ISystemClock systemClock, IOptions<WeightedBalancingStrategyOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.Value.Policies.Sum(x => x.Weight) is not 1.0D)
        {
            throw new ArgumentException("The sum of all weights must be 1.0D.", nameof(options));
        }

        _handles = options.Value.Policies
            .Select(x => new WeightedNodeBalancingStrategyItemHandle(systemClock, x.Policy, x.Weight))
            .ToImmutableArray();
    }

    public async ValueTask<NodeBalanceResult> ScoreAsync(ImmutableArray<ILavalinkNode> nodes, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var builder = ImmutableArray.CreateBuilder<ScoredLavalinkNode>(nodes.Length);
        var minimumDuration = TimeSpan.MaxValue;

        for (var nodeIndex = 0; nodeIndex < nodes.Length; nodeIndex++)
        {
            builder[nodeIndex] = new ScoredLavalinkNode(nodes[nodeIndex], 0.0D);
        }

        foreach (var handle in _handles)
        {
            var itemResult = await handle
                .GetAsync(nodes, cancellationToken)
                .ConfigureAwait(false);

            if (itemResult.Duration < minimumDuration)
            {
                minimumDuration = itemResult.Duration;
            }

            for (var nodeIndex = 0; nodeIndex < nodes.Length; nodeIndex++)
            {
                var itemScore = itemResult.Nodes[nodeIndex].Score * handle.Weight;
                var newScore = builder[nodeIndex].Score + itemScore;
                builder[nodeIndex] = builder[nodeIndex] with { Score = newScore, };
            }
        }

        return new NodeBalanceResult(builder.MoveToImmutable(), minimumDuration);
    }
}

internal sealed class WeightedNodeBalancingStrategyItemHandle
{
    private readonly ISystemClock _systemClock;
    private NodeBalanceResult _result;

    public WeightedNodeBalancingStrategyItemHandle(ISystemClock systemClock, INodeBalancingStrategy policy, double weight)
    {
        ArgumentNullException.ThrowIfNull(systemClock);
        ArgumentNullException.ThrowIfNull(policy);

        _systemClock = systemClock;

        Policy = policy;
        Weight = weight;
    }

    public INodeBalancingStrategy Policy { get; }

    public double Weight { get; }

    public DateTimeOffset RefreshAt { get; private set; }

    public bool IsExpired => _systemClock.UtcNow >= RefreshAt;

    public async ValueTask<NodeBalanceResult> GetAsync(ImmutableArray<ILavalinkNode> nodes, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (IsExpired)
        {
            _result = await Policy
                .ScoreAsync(nodes, cancellationToken)
                .ConfigureAwait(false);

            RefreshAt = _systemClock.UtcNow + _result.Duration;
        }

        return _result;
    }
}