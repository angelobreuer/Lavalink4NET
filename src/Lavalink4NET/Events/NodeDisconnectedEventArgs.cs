namespace Lavalink4NET.Events;

using System;
using System.Net.WebSockets;
using Lavalink4NET.Cluster;

/// <summary>
///     The event arguments for the <see cref="LavalinkCluster.NodeDisconnected"/> event.
/// </summary>
public class NodeDisconnectedEventArgs : DisconnectedEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NodeDisconnectedEventArgs"/> class.
    /// </summary>
    /// <param name="node">the node that disconnected</param>
    /// <param name="uri">the URI connect / reconnected / disconnected from / to</param>
    /// <param name="closeStatus">the close status</param>
    /// <param name="reason">the close reason</param>
    /// <param name="byRemote">
    ///     a value indicating whether the connection was closed by the remote endpoint.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="uri"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="node"/> is <see langword="null"/>.
    /// </exception>
    public NodeDisconnectedEventArgs(LavalinkClusterNode node, Uri uri, WebSocketCloseStatus closeStatus, string? reason, bool byRemote)
        : base(uri, closeStatus, reason, byRemote) => Node = node ?? throw new ArgumentNullException(nameof(node));

    /// <summary>
    ///     Initializes a new instance of the <see cref="NodeDisconnectedEventArgs"/> class.
    /// </summary>
    /// <param name="node">the node that connected</param>
    /// <param name="eventArgs">the event arguments to copy</param>
    public NodeDisconnectedEventArgs(LavalinkClusterNode node, DisconnectedEventArgs eventArgs)
        : base(eventArgs.Uri, eventArgs.CloseStatus, eventArgs.Reason, eventArgs.ByRemote)
        => Node = node ?? throw new ArgumentNullException(nameof(node));

    /// <summary>
    ///     Gets the node that disconnected.
    /// </summary>
    public LavalinkClusterNode Node { get; }
}
