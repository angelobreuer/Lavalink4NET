namespace Lavalink4NET.Cluster;

using System.Collections.Immutable;
using Lavalink4NET.Cluster.Nodes;
using Lavalink4NET.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class LavalinkClusterNode : ILavalinkNode
{
    private readonly LavalinkNode _node;
    private readonly CancellationToken _shutdownCancellationToken;
    private readonly CancellationTokenSource _stopCancellationTokenSource;
    private Task? _task;

    public LavalinkClusterNode(
        ILavalinkCluster cluster,
        ILavalinkApiClient apiClient,
        LavalinkNodeServiceContext serviceContext,
        IOptions<LavalinkClusterNodeOptions> options,
        CancellationToken shutdownCancellationToken,
        ILogger<LavalinkNode> logger)
    {
        ArgumentNullException.ThrowIfNull(cluster);

        _node = new LavalinkNode(serviceContext, options, apiClient.Endpoints, logger);

        Tags = options.Value.Tags;
        Metadata = options.Value.Metadata;
        Cluster = cluster;
        ApiClient = apiClient;

        _stopCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(shutdownCancellationToken);
        _shutdownCancellationToken = _stopCancellationTokenSource.Token;
    }

    public string? SessionId => _node.SessionId;

    public string Label => _node.Label;

    public ImmutableArray<string> Tags { get; }

    public IImmutableDictionary<string, string> Metadata { get; }

    public ILavalinkCluster Cluster { get; }

    public LavalinkNodeStatus Status
    {
        get
        {
            if (_task is null)
            {
                return LavalinkNodeStatus.OnDemand;
            }

            if (!_node.IsReady)
            {
                return LavalinkNodeStatus.WaitingForReady;
            }

            if (_task.Status is not TaskStatus.Running)
            {
                return LavalinkNodeStatus.Unavailable;
            }

            return LavalinkNodeStatus.Available; // TODO: Check if degraded
        }
    }

    public ILavalinkApiClient ApiClient { get; }

    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _task ??= _node.RunAsync(_shutdownCancellationToken).AsTask();

        return default;
    }

    public async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _stopCancellationTokenSource.Cancel();

        if (_task is not null)
        {
            await _task
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            _task = null;
        }
    }

    public ValueTask WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _node.WaitForReadyAsync(cancellationToken);
    }
}
