namespace Lavalink4NET.Cluster.Events;

using System;
using Lavalink4NET.Cluster.Nodes;

public sealed class NodeStatusChangedEventArgs : EventArgs
{
    public NodeStatusChangedEventArgs(ILavalinkNode node, LavalinkNodeStatus previousStatus, LavalinkNodeStatus currentStatus)
    {
        ArgumentNullException.ThrowIfNull(node);

        Node = node;
        PreviousStatus = previousStatus;
        CurrentStatus = currentStatus;
    }

    public ILavalinkNode Node { get; }

    public LavalinkNodeStatus PreviousStatus { get; }

    public LavalinkNodeStatus CurrentStatus { get; }
}
