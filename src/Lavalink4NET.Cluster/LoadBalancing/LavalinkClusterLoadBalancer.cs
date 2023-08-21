namespace Lavalink4NET.Cluster.LoadBalancing;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Cluster.LoadBalancing.Strategies;
using Lavalink4NET.Cluster.Nodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

internal sealed class LavalinkClusterLoadBalancer : ILavalinkClusterLoadBalancer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISystemClock _systemClock;
    private readonly INodeBalancingStrategy _nodeBalancingStrategy;
    private ILavalinkNode? _primaryNode;
    private ILavalinkCluster? _lavalinkCluster;

    public LavalinkClusterLoadBalancer(IServiceProvider serviceProvider, ISystemClock systemClock, INodeBalancingStrategy nodeBalancingStrategy)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(systemClock);
        ArgumentNullException.ThrowIfNull(nodeBalancingStrategy);

        _serviceProvider = serviceProvider;
        _systemClock = systemClock;
        _nodeBalancingStrategy = nodeBalancingStrategy;
    }

    public DateTimeOffset RefreshAt { get; private set; }

    public async ValueTask<ILavalinkNode> GetPreferredNodeAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_systemClock.UtcNow >= RefreshAt)
        {
            _lavalinkCluster ??= _serviceProvider.GetRequiredService<ILavalinkCluster>();

            var balanceResult = await _nodeBalancingStrategy
                .ScoreAsync(_lavalinkCluster.Nodes, cancellationToken)
                .ConfigureAwait(false);

            var primaryNode = balanceResult.Nodes
                .OrderByDescending(x => x.Score)
                .FirstOrDefault()
                .Node;

            _primaryNode = primaryNode;
            RefreshAt = _systemClock.UtcNow + balanceResult.Duration;

            await primaryNode
                .StartAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        return _primaryNode ?? throw new InvalidOperationException("No node is currently available according to the node balancing strategy.");
    }
}
