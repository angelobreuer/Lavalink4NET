/*
 *  File:   PlayerMovedEventArgs.cs
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

namespace Lavalink4NET.Events
{
    using System;
    using Lavalink4NET.Cluster;
    using Lavalink4NET.Player;

    /// <summary>
    ///     The <see cref="EventArgs"/> for the <see cref="LavalinkCluster.PlayerMoved"/> event.
    /// </summary>
    public sealed class PlayerMovedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerMovedEventArgs"/> class.
        /// </summary>
        /// <param name="node">the node the player was dropped from</param>
        /// <param name="targetNode">
        ///     the node the player was moved to; if <see langword="null"/> the player was not moved
        ///     to a new node, because no node was available
        /// </param>
        /// <param name="player">the player that was dropped</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="node"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="player"/> is <see langword="null"/>.
        /// </exception>
        public PlayerMovedEventArgs(LavalinkNode node, LavalinkNode targetNode, LavalinkPlayer player)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            Player = player ?? throw new ArgumentNullException(nameof(player));
            TargetNode = targetNode;
        }

        /// <summary>
        ///     Gets a value indicating whether the player could be moved to another node.
        /// </summary>
        public bool CouldBeMoved => TargetNode != null;

        /// <summary>
        ///     Gets the node the player was dropped from.
        /// </summary>
        public LavalinkNode Node { get; }

        /// <summary>
        ///     Gets the player that was dropped.
        /// </summary>
        public LavalinkPlayer Player { get; }

        /// <summary>
        ///     Gets the node the player was moved to; if <see langword="null"/> the player was not
        ///     moved to a new node, because no node was available.
        /// </summary>
        public LavalinkNode TargetNode { get; }
    }
}