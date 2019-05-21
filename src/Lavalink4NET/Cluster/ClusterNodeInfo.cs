/* 
 *  File:   ClusterNodeInfo.cs
 *  Author: Angelo Breuer
 *  
 *  The MIT License (MIT)
 *  
 *  Copyright (c) Angelo Breuer 2019
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

using System;
using Lavalink4NET.Events;

namespace Lavalink4NET.Cluster
{
    /// <summary>
    ///     Stores information about a cluster node.
    /// </summary>
    public sealed class ClusterNodeInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ClusterNodeInfo"/> class.
        /// </summary>
        /// <param name="node">the node</param>
        /// <param name="id">the node number</param>
        public ClusterNodeInfo(LavalinkNode node, int id = 0)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            LastUsage = DateTimeOffset.MinValue;
            Identifier = "Cluster Node-" + id;
        }

        /// <summary>
        ///     Gets the target node.
        /// </summary>
        public LavalinkNode Node { get; }

        /// <summary>
        ///     Gets an identifier that is used to identify the node (used for debugging or logging).
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        ///     Gets the node statistics (may be <see langword="null"/>).
        /// </summary>
        public StatisticUpdateEventArgs Statistics { get; internal set; }

        /// <summary>
        ///     Gets the coordinated universal time (UTC) point of the last usage of the node.
        /// </summary>
        public DateTimeOffset LastUsage { get; internal set; }
    }
}