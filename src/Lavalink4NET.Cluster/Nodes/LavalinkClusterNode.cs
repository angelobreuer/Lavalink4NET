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
    private readonly object _taskSyncRoot;
    private readonly ILavalinkClusterNodeListener? _clusterNodeListener;
    private int _status;
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

        _node = new LavalinkNode(
            serviceContext: serviceContext,
            apiClient: apiClient,
            options: options,
            apiEndpoints: apiClient.Endpoints,
            logger: logger);

        Tags = options.Value.Tags;
        Metadata = options.Value.Metadata;
        Cluster = cluster;
        ApiClient = apiClient;

        _status = (int)LavalinkNodeStatus.OnDemand;
        _readyTask = readyTask;
        _taskSyncRoot = new object();
        _stopCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(shutdownCancellationToken);
        _shutdownCancellationToken = _stopCancellationTokenSource.Token;
        _clusterNodeListener = serviceContext.NodeListener as ILavalinkClusterNodeListener;
    }

    public string? SessionId => _node.SessionId;

    public string Label => _node.Label;

    public ImmutableArray<string> Tags { get; }

    public IImmutableDictionary<string, string> Metadata { get; }

    public ILavalinkCluster Cluster { get; }

    public LavalinkNodeStatus Status => (LavalinkNodeStatus)_status;

    public ILavalinkApiClient ApiClient { get; }

    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_taskSyncRoot)
        {
            if (_task is null || Status is LavalinkNodeStatus.OnDemand)
            {
                _task = RunInternalAsync(_shutdownCancellationToken);
            }

            if (_task.IsCompleted)
            {
                return new ValueTask(_task);
            }
        }

        return default;
    }

    public async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _stopCancellationTokenSource.Cancel();

        Task? task;
        lock (_taskSyncRoot)
        {
            task = _task;
            _task = null;
        }

        if (task is not null)
        {
            await task
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    public ValueTask WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _node.WaitForReadyAsync(cancellationToken);
    }

    private async ValueTask UpdateStatusAsync(LavalinkNodeStatus nodeStatus, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var previousStatus = (LavalinkNodeStatus)Interlocked.Exchange(ref _status, (int)nodeStatus);

        if (previousStatus == nodeStatus)
        {
            return;
        }

        if (_clusterNodeListener is not null)
        {
            await _clusterNodeListener
                .OnStatusChangedAsync(this, previousStatus, nodeStatus, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private async Task RunInternalAsync(CancellationToken shutdownCancellationToken = default)
    {
        shutdownCancellationToken.ThrowIfCancellationRequested();

        try
        {
            await UpdateStatusAsync(LavalinkNodeStatus.WaitingForReady, shutdownCancellationToken);

            var clientInformation = await _readyTask
                .WaitAsync(shutdownCancellationToken)
                .ConfigureAwait(false);

            var nodeTask = _node.RunAsync(clientInformation, _shutdownCancellationToken).AsTask();

            await _node
                .WaitForReadyAsync(shutdownCancellationToken)
                .ConfigureAwait(false);

            await UpdateStatusAsync(LavalinkNodeStatus.Available, shutdownCancellationToken);

            await nodeTask.ConfigureAwait(false);
        }
        finally
        {
            var exitStatus = _shutdownCancellationToken.IsCancellationRequested
                ? LavalinkNodeStatus.OnDemand
                : LavalinkNodeStatus.Unavailable;

            await UpdateStatusAsync(exitStatus, shutdownCancellationToken);
        }
    }
}
