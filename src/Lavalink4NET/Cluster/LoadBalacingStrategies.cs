/* 
 *  File:   LoadBalacingStrategies.cs
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
    using System.Linq;

    /// <summary>
    ///     Provides a set of load balancing strategies.
    /// </summary>
    public static class LoadBalacingStrategies
    {
        /// <summary>
        ///     The round robin load balancing strategy favors the node that has not been used the longest.
        /// </summary>
        public static LoadBalacingStrategy RoundRobinStrategy { get; } = (cluster, nodes) =>
            nodes.OrderBy(s => s.LastUsage).First();

        /// <summary>
        ///     The load-balancing strategy for the fewest players uses the node that has the
        ///     least-playing player.
        /// </summary>
        public static LoadBalacingStrategy LeastPlayersStrategy { get; } = (cluster, nodes) =>
            nodes.OrderBy(s => s.Statistics?.PlayingPlayers ?? 0).First();
    }
}