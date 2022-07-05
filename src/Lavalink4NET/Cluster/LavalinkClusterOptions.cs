/*
 *  File:   LavalinkClusterOptions.cs
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

namespace Lavalink4NET.Cluster;

using System;
using System.Collections.Generic;

/// <summary>
///     The options for a lavalink cluster ( <see cref="LavalinkCluster"/>).
/// </summary>
public sealed class LavalinkClusterOptions
{
    /// <summary>
    ///     Gets or sets the load balancing strategy to use.
    /// </summary>
    public LoadBalancingStrategy LoadBalacingStrategy { get; set; }
        = LoadBalancingStrategies.ScoreStrategy;

    /// <summary>
    ///     Gets or sets the cluster node options.
    /// </summary>
    public IReadOnlyList<LavalinkNodeOptions> Nodes { get; set; }
        = Array.Empty<LavalinkNodeOptions>();

    /// <summary>
    ///     Gets or sets cluster node factory.
    /// </summary>
    public ClusterNodeFactory<LavalinkClusterNode> NodeFactory { get; set; }
        = LavalinkClusterNode.ClusterNodeFactory;

    /// <summary>
    ///     Gets or sets a value indicating whether players should be moved to a new node if a
    ///     node disconnects accidentally.
    /// </summary>
    /// <remarks>
    ///     This property defaults to <see langword="false"/>, because this feature is very experimental.
    /// </remarks>
    public bool StayOnline { get; set; } = false;
}
