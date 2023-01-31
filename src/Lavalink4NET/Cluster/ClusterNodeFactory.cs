namespace Lavalink4NET.Cluster;

using Lavalink4NET.Integrations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;


/// <summary>
///     A factory used to create cluster nodes.
/// </summary>
/// <param name="cluster">the cluster</param>
/// <param name="options">the node options for connecting</param>
/// <param name="client">the discord client</param>
/// <param name="logger">the logger</param>
/// <param name="cache">an optional cache that caches track requests</param>
/// <param name="id">the node number</param>
public delegate LavalinkClusterNode ClusterNodeFactory(
    LavalinkCluster cluster,
    LavalinkNodeOptions options,
    IDiscordClientWrapper client,
    int id,
    IIntegrationCollection integrationCollection,
    ILogger<LavalinkClusterNode>? logger = null,
    IMemoryCache? cache = null);