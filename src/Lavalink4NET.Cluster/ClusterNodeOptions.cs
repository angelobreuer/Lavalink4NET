namespace Lavalink4NET.Cluster;

public sealed record class ClusterNodeOptions
{
    public string Passphrase { get; set; } = "youshallnotpass";

    public Uri? WebSocketUri { get; set; }


}
