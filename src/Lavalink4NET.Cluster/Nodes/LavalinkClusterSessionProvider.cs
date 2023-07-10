namespace Lavalink4NET.Cluster.Nodes;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Cluster.LoadBalancing;
using Lavalink4NET.Players;

internal sealed class LavalinkClusterSessionProvider : ILavalinkSessionProvider
{
    private readonly ILavalinkClusterLoadBalancer _loadBalancer;

    public LavalinkClusterSessionProvider(ILavalinkClusterLoadBalancer loadBalancer)
    {
        ArgumentNullException.ThrowIfNull(loadBalancer);

        _loadBalancer = loadBalancer;
    }

    public async ValueTask<LavalinkPlayerSession> GetSessionIdAsync(ulong guildId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var node = await _loadBalancer
            .GetPreferredNodeAsync(cancellationToken)
            .ConfigureAwait(false);

        await node
            .WaitForReadyAsync(cancellationToken)
            .ConfigureAwait(false);

        return new LavalinkPlayerSession(node.ApiClient, node.SessionId!);
    }
}
