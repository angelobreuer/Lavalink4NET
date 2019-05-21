/* 
 *  File:   LavalinkClusterNode.cs
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

namespace Lavalink4NET.Cluster
{
    using System.Threading.Tasks;
    using Events;
    using Microsoft.Extensions.Logging;

    internal sealed class LavalinkClusterNode : LavalinkNode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LavalinkNode"/> class.
        /// </summary>
        /// <param name="cluster">the cluster</param>
        /// <param name="options">the node options for connecting</param>
        /// <param name="client">the discord client</param>
        /// <param name="logger">the logger</param>
        public LavalinkClusterNode(LavalinkCluster cluster, LavalinkNodeOptions options, IDiscordClientWrapper client, ILogger<Lavalink> logger)
            : base(options, client, logger) => Cluster = cluster;

        /// <summary>
        ///     Gets the cluster owning the node.
        /// </summary>
        public LavalinkCluster Cluster { get; }

        /// <summary>
        ///     Invokes the <see cref="LavalinkNode.StatisticsUpdated"/> event asynchronously.
        /// </summary>
        /// <param name="eventArgs">the event arguments</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        protected override Task OnStatisticsUpdateAsync(StatisticUpdateEventArgs eventArgs)
        {
            Cluster.ReportStatistics(this, eventArgs);
            return base.OnStatisticsUpdateAsync(eventArgs);
        }
    }
}