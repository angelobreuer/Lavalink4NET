namespace Lavalink4NET.Cluster.Nodes;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Rest;

public interface ILavalinkNode
{
    ILavalinkApiClient ApiClient { get; }

    ILavalinkCluster Cluster { get; }

    string? SessionId { get; }

    ImmutableArray<string> Tags { get; }

    IImmutableDictionary<string, string> Metadata { get; }

    LavalinkNodeStatus Status { get; }

    ValueTask WaitForReadyAsync(CancellationToken cancellationToken = default);

    ValueTask StartAsync(CancellationToken cancellationToken = default);

    ValueTask StopAsync(CancellationToken cancellationToken = default);
}
