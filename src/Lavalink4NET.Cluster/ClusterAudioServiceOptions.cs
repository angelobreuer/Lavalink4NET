namespace Lavalink4NET.Cluster;

using System;
using System.Collections.Immutable;
using Lavalink4NET.Cluster.Nodes;
using Microsoft.Extensions.Options;

public sealed record class ClusterAudioServiceOptions
{
    public string HttpClientName { get; set; } = Options.DefaultName;

    public TimeSpan ReadyTimeout { get; set; } = TimeSpan.FromSeconds(10);

    public ImmutableArray<LavalinkClusterNodeOptions> Nodes { get; set; } = ImmutableArray.Create(new LavalinkClusterNodeOptions());
}
