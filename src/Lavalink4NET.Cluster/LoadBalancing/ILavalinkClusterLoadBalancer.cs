namespace Lavalink4NET.Cluster.LoadBalancing;

using System;
using System.Threading.Tasks;
using Lavalink4NET.Cluster.Nodes;

public interface ILavalinkClusterLoadBalancer
{
    DateTimeOffset RefreshAt { get; }

    ValueTask<ILavalinkNode> GetPreferredNodeAsync(CancellationToken cancellationToken = default);
}
