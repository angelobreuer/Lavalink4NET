/* 
 *  File:   LavalinkCluster.cs
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Events;
    using Microsoft.Extensions.Logging;
    using Player;
    using Rest;

    /// <summary>
    ///     A set of lavalink nodes bound to a cluster usable for voice node balancing.
    /// </summary>
    public sealed class LavalinkCluster : IAudioService, ILavalinkRestClient, IDisposable
    {
        private readonly LoadBalacingStrategy _loadBalacingStrategy;
        private readonly List<ClusterNodeInfo> _nodes;
        private readonly IDiscordClientWrapper _client;
        private readonly ILogger<Lavalink> _logger;
        private volatile int _nodeId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LavalinkCluster"/> class.
        /// </summary>
        /// <param name="options">the cluster options</param>
        /// <param name="client">the discord client</param>
        /// <param name="logger">the logger</param>
        public LavalinkCluster(LavalinkClusterOptions options, IDiscordClientWrapper client, ILogger<Lavalink> logger = null)
        {
            _loadBalacingStrategy = options.LoadBalacingStrategy;
            _client = client;
            _logger = logger;
            _nodes = options.Nodes.Select(CreateNodeInfo).ToList();
        }

        /// <summary>
        ///     Initializes the audio service asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        public Task InitializeAsync() => Task.WhenAll(_nodes.Select(s => s.Node.InitializeAsync()));

        /// <summary>
        ///     Creates a node info for the specified <paramref name="nodeOptions"/>.
        /// </summary>
        /// <param name="nodeOptions">the node options</param>
        /// <returns>the created node info</returns>
        private ClusterNodeInfo CreateNodeInfo(LavalinkNodeOptions nodeOptions)
            => new ClusterNodeInfo(new LavalinkClusterNode(this, nodeOptions, _client, _logger), _nodeId++);

        /// <summary>
        ///     Dynamically adds a node to the cluster asynchronously.
        /// </summary>
        /// <param name="nodeOptions">the node connection options</param>
        /// <returns>
        ///     a task that represents the asynchronous operation
        ///     <para>the cluster node info created for the node</para>
        /// </returns>
        public async Task<ClusterNodeInfo> AddNodeAsync(LavalinkNodeOptions nodeOptions)
        {
            // create node info
            var info = CreateNodeInfo(nodeOptions);

            // initialize node
            await info.Node.InitializeAsync();

            // add node info to nodes (so make it available for load balancing)
            _nodes.Add(info);

            return info;
        }

        /// <summary>
        ///     Gets the preferred node using the <see cref="LoadBalacingStrategy"/> specified in the options.
        /// </summary>
        public LavalinkNode GetPreferredNode()
        {
            // get the preferred node by the load balancing strategy
            var node = _loadBalacingStrategy(this, _nodes.ToArray());

            // update last usage
            node.LastUsage = DateTimeOffset.UtcNow;

            return node.Node;
        }

        /// <summary>
        ///     Disposes all underlying nodes.
        /// </summary>
        public void Dispose() => _nodes.ForEach(s => s.Node.Dispose());

        /// <summary>
        ///     Gets the audio player for the specified <paramref name="guildId"/>.
        /// </summary>
        /// <typeparam name="TPlayer">the type of the player to use</typeparam>
        /// <param name="guildId">the guild identifier to get the player for</param>
        /// <returns>the player for the guild</returns>
        public TPlayer GetPlayer<TPlayer>(ulong guildId) where TPlayer : LavalinkPlayer
            => GetServingNode(guildId).GetPlayer<TPlayer>(guildId);

        /// <summary>
        ///     Gets the node that serves the guild specified by <paramref name="guildId"/> (if no
        ///     node serves the guild, <see cref="GetPreferredNode()"/> is used).
        /// </summary>
        /// <param name="guildId">the guild snowflake identifier</param>
        /// <returns></returns>
        public LavalinkNode GetServingNode(ulong guildId)
            => _nodes.FirstOrDefault(s => s.Node.HasPlayer(guildId))?.Node ?? GetPreferredNode();

        /// <summary>
        ///     Gets the track for the specified <paramref name="query"/> asynchronously.
        /// </summary>
        /// <param name="query">the track search query</param>
        /// <param name="mode">the track search mode</param>
        /// <returns>the track found for the query</returns>
        public Task<LavalinkTrack> GetTrackAsync(string query, SearchMode mode = SearchMode.None)
            => GetPreferredNode().GetTrackAsync(query, mode);

        /// <summary>
        ///     Gets the tracks for the specified <paramref name="query"/> asynchronously.
        /// </summary>
        /// <param name="query">the track search query</param>
        /// <param name="mode">the track search mode</param>
        /// <returns>the tracks found for the query</returns>
        public Task<IEnumerable<LavalinkTrack>> GetTracksAsync(string query, SearchMode mode = SearchMode.None)
            => GetPreferredNode().GetTracksAsync(query, mode);

        /// <summary>
        ///     Gets a value indicating whether a player is created for the specified <paramref name="guildId"/>.
        /// </summary>
        /// <param name="guildId">
        ///     the snowflake identifier of the guild to create the player for
        /// </param>
        /// <returns>a value indicating whether a player is created for the specified <paramref name="guildId"/></returns>
        public bool HasPlayer(ulong guildId)
            => _nodes.Any(s => s.Node.HasPlayer(guildId));

        /// <summary>
        ///     Joins the channel specified by <paramref name="voiceChannelId"/> asynchronously.
        /// </summary>
        /// <param name="guildId">the guild snowflake identifier</param>
        /// <param name="voiceChannelId">the snowflake identifier of the voice channel to join</param>
        /// <param name="selfDeaf">a value indicating whether the bot user should be self deafened</param>
        /// <param name="selfMute">a value indicating whether the bot user should be self muted</param>
        /// <returns>
        ///     a task that represents the asynchronous operation
        ///     <para>the audio player</para>
        /// </returns>
        public Task<TPlayer> JoinAsync<TPlayer>(ulong guildId, ulong voiceChannelId, bool selfDeaf = false, bool selfMute = false)
            where TPlayer : LavalinkPlayer
            => GetServingNode(guildId).JoinAsync<TPlayer>(guildId, voiceChannelId, selfDeaf, selfMute);

        /// <summary>
        ///     Loads the tracks specified by the <paramref name="query"/> asynchronously.
        /// </summary>
        /// <param name="query">the search query</param>
        /// <param name="mode">the track search mode</param>
        /// <returns>
        ///     a task that represents the asynchronous operation <param>the request response</param>
        /// </returns>
        public Task<TrackLoadResponsePayload> LoadTracksAsync(string query, SearchMode mode = SearchMode.None)
            => GetPreferredNode().LoadTracksAsync(query, mode);

        /// <summary>
        ///     Reports the statistics for the specified <paramref name="clusterNode"/>.
        /// </summary>
        /// <param name="clusterNode">the cluster node the report is for</param>
        /// <param name="eventArgs">the reported statistics</param>
        internal void ReportStatistics(LavalinkClusterNode clusterNode, StatisticUpdateEventArgs eventArgs)
            => _nodes.Single(s => s.Node == clusterNode).Statistics = eventArgs;
    }
}