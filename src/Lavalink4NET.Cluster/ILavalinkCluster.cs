namespace Lavalink4NET.Cluster;

using System.Collections.Immutable;
using Lavalink4NET.Cluster.Nodes;

public interface ILavalinkCluster
{
    ImmutableArray<ILavalinkNode> Nodes { get; }
}
