namespace Lavalink4NET.Cluster;

using System.Collections.Generic;

/// <summary>
///     Gets a suitable node using the load balancing strategy.
/// </summary>
/// <param name="cluster">the cluster</param>
/// <param name="nodes">the nodes</param>
/// <param name="type">the type of the purpose for the node</param>
/// <returns>the preferred node</returns>
public delegate LavalinkClusterNode LoadBalancingStrategy(
    LavalinkCluster cluster, IReadOnlyCollection<LavalinkClusterNode> nodes,
    NodeRequestType type = NodeRequestType.Unspecified);
