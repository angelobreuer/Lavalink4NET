namespace Lavalink4NET.Cluster;

using System;
using System.Threading.Tasks;
using Events;
using Lavalink4NET.Integrations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

/// <summary>
///     A clustered lavalink node with additional information.
/// </summary>
public class LavalinkClusterNode : LavalinkNode
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LavalinkNode"/> class.
    /// </summary>
    /// <param name="cluster">the cluster</param>
    /// <param name="options">the node options for connecting</param>
    /// <param name="client">the discord client</param>
    /// <param name="logger">the logger</param>
    /// <param name="cache">an optional cache that caches track requests</param>
    /// <param name="id">the node number</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="cluster"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="options"/> parameter is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="client"/> is <see langword="null"/>.
    /// </exception>
    public LavalinkClusterNode(
        LavalinkCluster cluster,
        LavalinkNodeOptions options,
        IDiscordClientWrapper client,
        int id,
        IIntegrationCollection integrationCollection,
        ILogger<LavalinkClusterNode>? logger = null,
        IMemoryCache? cache = null)
        : base(options, client, logger, cache)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (Label is null)
        {
            Label = "Cluster Node-" + id;
        }

        Id = id;
        Cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));
        LastUsage = DateTimeOffset.MinValue;
        Integrations = integrationCollection;
    }

    /// <summary>
    ///     Gets the <see cref="ClusterNodeFactory"/> for this type.
    /// </summary>
    public static ClusterNodeFactory ClusterNodeFactory { get; } =
        (cluster, options, client, id, integrationCollection, logger, cache) =>
            new LavalinkClusterNode(cluster, options, client, id, integrationCollection, logger, cache);

    /// <summary>
    ///     Gets the cluster owning the node.
    /// </summary>
    public LavalinkCluster Cluster { get; }

    /// <summary>
    ///     Gets the cluster node id.
    /// </summary>
    public int Id { get; }

    /// <summary>
    ///     Gets the coordinated universal time (UTC) point of the last usage of the node.
    /// </summary>
    public DateTimeOffset LastUsage { get; internal set; }

    /// <inheritdoc/>
    protected override Task OnConnectedAsync(ConnectedEventArgs eventArgs)
        => Task.WhenAll(base.OnConnectedAsync(eventArgs), Cluster.NodeConnectedAsync(this, eventArgs));

    /// <inheritdoc/>
    protected override Task OnDisconnectedAsync(DisconnectedEventArgs eventArgs)
        => Task.WhenAll(base.OnDisconnectedAsync(eventArgs), Cluster.NodeDisconnectedAsync(this, eventArgs));

    /// <inheritdoc/>
    protected override Task OnTrackEndAsync(TrackEndEventArgs eventArgs)
        => Task.WhenAll(base.OnTrackEndAsync(eventArgs), Cluster.OnTrackEndAsync(eventArgs));

    /// <inheritdoc/>
    protected override Task OnTrackExceptionAsync(TrackExceptionEventArgs eventArgs)
        => Task.WhenAll(base.OnTrackExceptionAsync(eventArgs), Cluster.OnTrackExceptionAsync(eventArgs));

    /// <inheritdoc/>
    protected override Task OnTrackStartedAsync(TrackStartedEventArgs eventArgs)
        => Task.WhenAll(base.OnTrackStartedAsync(eventArgs), Cluster.OnTrackStartedAsync(eventArgs));

    /// <inheritdoc/>
    protected override Task OnTrackStuckAsync(TrackStuckEventArgs eventArgs)
        => Task.WhenAll(base.OnTrackStuckAsync(eventArgs), Cluster.OnTrackStuckAsync(eventArgs));
}
