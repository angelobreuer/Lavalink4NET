namespace Lavalink4NET.Cluster.Nodes;

public enum LavalinkNodeStatus : byte
{
    OnDemand,
    WaitingForReady,
    Available,
    Unavailable,
    Degraded,
}
