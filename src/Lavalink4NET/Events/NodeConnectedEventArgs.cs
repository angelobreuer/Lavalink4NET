namespace Lavalink4NET.Events;

using System;
using Lavalink4NET.Cluster;

/// <summary>
///     The event arguments for the <see cref="LavalinkCluster.NodeConnected"/> event.
/// </summary>
public class NodeConnectedEventArgs : ConnectedEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NodeConnectedEventArgs"/> class.
    /// </summary>
    /// <param name="node">the node that connected</param>
    /// <param name="uri">the URI connect / reconnected / disconnected from / to</param>
    /// <param name="wasReconnect">a value indicating whether the connect was a reconnect</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="uri"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="node"/> is <see langword="null"/>.
    /// </exception>
    public NodeConnectedEventArgs(LavalinkClusterNode node, Uri uri, bool wasReconnect)
        : base(uri, wasReconnect) => Node = node ?? throw new ArgumentNullException(nameof(node));

    /// <summary>
    ///     Initializes a new instance of the <see cref="NodeConnectedEventArgs"/> class.
    /// </summary>
    /// <param name="node">the node that connected</param>
    /// <param name="eventArgs">the event arguments to copy</param>
    public NodeConnectedEventArgs(LavalinkClusterNode node, ConnectedEventArgs eventArgs)
        : base(eventArgs.Uri, eventArgs.WasReconnect)
        => Node = node ?? throw new ArgumentNullException(nameof(node));

    /// <summary>
    ///     Gets the node that connected.
    /// </summary>
    public LavalinkClusterNode Node { get; }
}
