/*
 *  File:   LavalinkClusterNode.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2020
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

namespace Lavalink4NET.Cluster
{
    using System;
    using System.Threading.Tasks;
    using Events;
    using Lavalink4NET.Logging;

    /// <summary>
    ///     A clustered lavalink node with additional information.
    /// </summary>
    public sealed class LavalinkClusterNode : LavalinkNode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LavalinkNode"/> class.
        /// </summary>
        /// <param name="cluster">the cluster</param>
        /// <param name="options">the node options for connecting</param>
        /// <param name="client">the discord client</param>
        /// <param name="logger">the logger</param>
        /// <param name="cache">an optional cache that caches track requests</param>
        /// <param name="id">the node number</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="cluster"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="options"/> parameter is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="client"/> is <see langword="null"/>.
        /// </exception>
        public LavalinkClusterNode(LavalinkCluster cluster, LavalinkNodeOptions options, IDiscordClientWrapper client,
            ILogger logger, ILavalinkCache cache, int id) : base(options, client, logger, cache)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            Cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));
            Identifier = "Cluster Node-" + id;
            LastUsage = DateTimeOffset.MinValue;
        }

        /// <summary>
        ///     Gets the cluster owning the node.
        /// </summary>
        public LavalinkCluster Cluster { get; }

        /// <summary>
        ///     Gets an identifier that is used to identify the node (used for debugging or logging).
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        ///     Gets the coordinated universal time (UTC) point of the last usage of the node.
        /// </summary>
        public DateTimeOffset LastUsage { get; internal set; }

        /// <summary>
        ///     Triggers the <see cref="LavalinkSocket.Connected"/> event asynchronously.
        /// </summary>
        /// <param name="eventArgs">the event arguments</param>
        /// <returns>a task that represents the asynchronously operation.</returns>
        protected override Task OnConnectedAsync(ConnectedEventArgs eventArgs)
            => Task.WhenAll(base.OnConnectedAsync(eventArgs), Cluster.NodeConnectedAsync(this, eventArgs));

        /// <summary>
        ///     Triggers the <see cref="LavalinkSocket.Disconnected"/> event asynchronously.
        /// </summary>
        /// <param name="eventArgs">the event arguments</param>
        /// <returns>a task that represents the asynchronously operation.</returns>
        protected override Task OnDisconnectedAsync(DisconnectedEventArgs eventArgs)
            => Task.WhenAll(base.OnDisconnectedAsync(eventArgs), Cluster.NodeDisconnectedAsync(this, eventArgs));
    }
}