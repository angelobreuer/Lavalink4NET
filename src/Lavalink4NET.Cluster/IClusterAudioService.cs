namespace Lavalink4NET.Cluster;

using Lavalink4NET.Cluster.Nodes;
using Microsoft.Extensions.Options;

public interface IClusterAudioService : IAudioService, ILavalinkCluster
{
    ValueTask<ILavalinkNode> AddAsync(
        IOptions<LavalinkClusterNodeOptions> options,
        CancellationToken cancellationToken = default);

    ValueTask<bool> RemoveAsync(ILavalinkNode node, CancellationToken cancellationToken = default);
}
