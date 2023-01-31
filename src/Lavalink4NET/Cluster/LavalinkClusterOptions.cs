namespace Lavalink4NET.Cluster;

using System;
using System.Collections.Generic;

/// <summary>
///     The options for a lavalink cluster ( <see cref="LavalinkCluster"/>).
/// </summary>
public sealed class LavalinkClusterOptions
{
    /// <summary>
    ///     Gets or sets the load balancing strategy to use.
    /// </summary>
    public LoadBalancingStrategy LoadBalacingStrategy { get; set; }
        = LoadBalancingStrategies.ScoreStrategy;

    /// <summary>
    ///     Gets or sets the cluster node options.
    /// </summary>
    public IReadOnlyList<LavalinkNodeOptions> Nodes { get; set; }
        = Array.Empty<LavalinkNodeOptions>();

    /// <summary>
    ///     Gets or sets cluster node factory.
    /// </summary>
    public ClusterNodeFactory NodeFactory { get; set; }
        = LavalinkClusterNode.ClusterNodeFactory;

    /// <summary>
    ///     Gets or sets a value indicating whether players should be moved to a new node if a
    ///     node disconnects accidentally.
    /// </summary>
    /// <remarks>
    ///     This property defaults to <see langword="false"/>, because this feature is very experimental.
    /// </remarks>
    public bool StayOnline { get; set; } = false;
}
