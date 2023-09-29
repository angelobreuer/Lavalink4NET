namespace Lavalink4NET.Cluster;

using Lavalink4NET.Cluster.Events;
using Lavalink4NET.Cluster.Nodes;
using Lavalink4NET.Events;
using Microsoft.Extensions.Options;

public interface IClusterAudioService : IAudioService, ILavalinkCluster
{
    event AsyncEventHandler<NodeStatusChangedEventArgs>? NodeStatusChanged;

    ValueTask<ILavalinkNode> AddAsync(
        IOptions<LavalinkClusterNodeOptions> options,
        CancellationToken cancellationToken = default);

    ValueTask<bool> RemoveAsync(ILavalinkNode node, CancellationToken cancellationToken = default);
}
