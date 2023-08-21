namespace Lavalink4NET.Cluster.Nodes;

using System.Collections.Immutable;

public record class LavalinkClusterNodeOptions : LavalinkNodeOptions
{
    public ImmutableArray<string> Tags { get; init; } = ImmutableArray<string>.Empty;

    public IImmutableDictionary<string, string> Metadata { get; init; } = ImmutableDictionary<string, string>.Empty;
}
