/*
 *  File:   NodeDisconnectedEventArgs.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2022
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */

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
