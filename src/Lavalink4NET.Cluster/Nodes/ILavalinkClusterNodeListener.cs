namespace Lavalink4NET.Cluster.Nodes;

using System.Threading.Tasks;

internal interface ILavalinkClusterNodeListener : ILavalinkNodeListener
{
    ValueTask OnStatusChangedAsync(
        ILavalinkNode node,
        LavalinkNodeStatus previousStatus,
        LavalinkNodeStatus currentStatus,
        CancellationToken cancellationToken = default);
}
