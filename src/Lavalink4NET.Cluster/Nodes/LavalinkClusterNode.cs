namespace Lavalink4NET.Cluster;

using System.Collections.Immutable;
using Lavalink4NET.Clients;
using Lavalink4NET.Cluster.Nodes;
using Lavalink4NET.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class LavalinkClusterNode : ILavalinkNode
{
    private readonly LavalinkNode _node;
    private readonly CancellationToken _shutdownCancellationToken;
    private readonly CancellationTokenSource _stopCancellationTokenSource;
    private readonly Task<ClientInformation> _readyTask;
    private Task? _task;

    public LavalinkClusterNode(
        ILavalinkCluster cluster,
        ILavalinkApiClient apiClient,
        LavalinkNodeServiceContext serviceContext,
        IOptions<LavalinkClusterNodeOptions> options,
        CancellationToken shutdownCancellationToken,
        Task<ClientInformation> readyTask,
        ILogger<LavalinkNode> logger)
    {
        ArgumentNullException.ThrowIfNull(cluster);
        ArgumentNullException.ThrowIfNull(apiClient);
        ArgumentNullException.ThrowIfNull(serviceContext);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(readyTask);
        ArgumentNullException.ThrowIfNull(logger);

        _node = new LavalinkNode(serviceContext, options, apiClient.Endpoints, logger);

        Tags = options.Value.Tags;
        Metadata = options.Value.Metadata;
        Cluster = cluster;
        ApiClient = apiClient;
        _readyTask = readyTask;
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

    public async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var clientInformation = await _readyTask
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        _task ??= _node.RunAsync(clientInformation, _shutdownCancellationToken).AsTask();
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
