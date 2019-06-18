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
    using Microsoft.Extensions.Logging;
    using Player;
    using Rest;

    /// <summary>
    ///     A set of lavalink nodes bound to a cluster usable for voice node balancing.
    /// </summary>
    public sealed class LavalinkCluster : IAudioService, ILavalinkRestClient, IDisposable
    {
        private readonly ILavalinkCache _cache;
        private readonly IDiscordClientWrapper _client;
        private readonly LoadBalacingStrategy _loadBalacingStrategy;
        private readonly ILogger<Lavalink> _logger;
        private readonly List<LavalinkClusterNode> _nodes;
        private readonly object _nodesLock;
        private bool _initialized;
        private volatile int _nodeId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LavalinkCluster"/> class.
        /// </summary>
        /// <param name="options">the cluster options</param>
        /// <param name="client">the discord client</param>
        /// <param name="logger">the logger</param>
        /// <param name="cache">
        ///     a cache that is shared between the different lavalink rest clients. If the cache is
        ///     <see langword="null"/>, no cache will be used.
        /// </param>
        public LavalinkCluster(LavalinkClusterOptions options, IDiscordClientWrapper client, ILogger<Lavalink> logger = null, ILavalinkCache cache = null)
        {
            _loadBalacingStrategy = options.LoadBalacingStrategy;
            _client = client;
            _logger = logger;
            _cache = cache;
            _nodesLock = new object();
            _nodes = options.Nodes.Select(CreateNode).ToList();
        }

        /// <summary>
        ///     Dynamically adds a node to the cluster asynchronously.
        /// </summary>
        /// <param name="nodeOptions">the node connection options</param>
        /// <returns>
        ///     a task that represents the asynchronous operation
        ///     <para>the cluster node info created for the node</para>
        /// </returns>
        public async Task<LavalinkClusterNode> AddNodeAsync(LavalinkNodeOptions nodeOptions)
        {
            var node = CreateNode(nodeOptions);

            // initialize node
            await node.InitializeAsync();

            // add node info to nodes (so make it available for load balancing)
            lock (_nodesLock)
            {
                _nodes.Add(node);
            }

            return node;
        }

        /// <summary>
        ///     Disposes all underlying nodes.
        /// </summary>
        public void Dispose()
        {
            lock (_nodesLock)
            {
                _nodes.ForEach(s => s.Dispose());
            }
        }

        /// <summary>
        ///     Gets the audio player for the specified <paramref name="guildId"/>.
        /// </summary>
        /// <typeparam name="TPlayer">the type of the player to use</typeparam>
        /// <param name="guildId">the guild identifier to get the player for</param>
        /// <returns>the player for the guild</returns>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the cluster has not been initialized.
        /// </exception>
        public TPlayer GetPlayer<TPlayer>(ulong guildId) where TPlayer : LavalinkPlayer
            => GetServingNode(guildId).GetPlayer<TPlayer>(guildId);

        /// <summary>
        ///     Gets all players of the specified <typeparamref name="TPlayer"/>.
        /// </summary>
        /// <typeparam name="TPlayer">
        ///     the type of the players to get; use <see cref="LavalinkPlayer"/> to get all players
        /// </typeparam>
        /// <returns>the player list</returns>
        public IReadOnlyList<TPlayer> GetPlayers<TPlayer>()
            where TPlayer : LavalinkPlayer
        {
            lock (_nodesLock)
            {
                return _nodes
                    .SelectMany(s => s.GetPlayers<TPlayer>())
                    .ToList();
            }
        }

        /// <summary>
        ///     Gets the preferred node using the <see cref="LoadBalacingStrategy"/> specified in the options.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the cluster has not been initialized.
        /// </exception>
        public LavalinkNode GetPreferredNode()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("The cluster has not been initialized.");
            }

            lock (_nodesLock)
            {
                // find a connected node
                var nodes = _nodes.Where(s => s.IsConnected).ToArray();

                // get the preferred node by the load balancing strategy
                var node = _loadBalacingStrategy(this, nodes);

                // update last usage
                node.LastUsage = DateTimeOffset.UtcNow;

                return node;
            }
        }

        /// <summary>
        ///     Gets the node that serves the guild specified by <paramref name="guildId"/> (if no
        ///     node serves the guild, <see cref="GetPreferredNode()"/> is used).
        /// </summary>
        /// <param name="guildId">the guild snowflake identifier</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the cluster has not been initialized.
        /// </exception>
        public LavalinkNode GetServingNode(ulong guildId)
        {
            lock (_nodesLock)
            {
                var node = _nodes.FirstOrDefault(s => s.HasPlayer(guildId));

                if (node != null)
                {
                    return node;
                }
            }

            return GetPreferredNode();
        }

        /// <summary>
        ///     Gets the track for the specified <paramref name="query"/> asynchronously.
        /// </summary>
        /// <param name="query">the track search query</param>
        /// <param name="mode">the track search mode</param>
        /// <returns>the track found for the query</returns>
        /// <param name="noCache">
        ///     a value indicating whether the track should be returned from cache, if it is cached.
        ///     Note this parameter does only take any effect is a cache provider is specified in constructor.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the cluster has not been initialized.
        /// </exception>
        public Task<LavalinkTrack> GetTrackAsync(string query, SearchMode mode = SearchMode.None, bool noCache = false)
            => GetPreferredNode().GetTrackAsync(query, mode, noCache);

        /// <summary>
        ///     Gets the tracks for the specified <paramref name="query"/> asynchronously.
        /// </summary>
        /// <param name="query">the track search query</param>
        /// <param name="mode">the track search mode</param>
        /// <param name="noCache">
        ///     a value indicating whether the track should be returned from cache, if it is cached.
        ///     Note this parameter does only take any effect is a cache provider is specified in constructor.
        /// </param>
        /// <returns>the tracks found for the query</returns>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the cluster has not been initialized.
        /// </exception>
        public Task<IEnumerable<LavalinkTrack>> GetTracksAsync(string query, SearchMode mode = SearchMode.None, bool noCache = false)
            => GetPreferredNode().GetTracksAsync(query, mode, noCache);

        /// <summary>
        ///     Gets a value indicating whether a player is created for the specified <paramref name="guildId"/>.
        /// </summary>
        /// <param name="guildId">
        ///     the snowflake identifier of the guild to create the player for
        /// </param>
        /// <returns>a value indicating whether a player is created for the specified <paramref name="guildId"/></returns>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the cluster has not been initialized.
        /// </exception>
        public bool HasPlayer(ulong guildId)
        {
            lock (_nodesLock)
            {
                return _nodes.Any(s => s.HasPlayer(guildId));
            }
        }

        /// <summary>
        ///     Initializes all nodes asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        public async Task InitializeAsync()
        {
            await Task.WhenAll(_nodes.Select(s => s.InitializeAsync()));

            _initialized = true;
        }

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
        /// <exception cref="InvalidOperationException">
        ///     thrown if the cluster has not been initialized.
        /// </exception>
        public Task<TPlayer> JoinAsync<TPlayer>(ulong guildId, ulong voiceChannelId, bool selfDeaf = false, bool selfMute = false)
            where TPlayer : LavalinkPlayer
            => GetServingNode(guildId).JoinAsync<TPlayer>(guildId, voiceChannelId, selfDeaf, selfMute);

        /// <summary>
        ///     Loads the tracks specified by the <paramref name="query"/> asynchronously.
        /// </summary>
        /// <param name="query">the search query</param>
        /// <param name="mode">the track search mode</param>
        /// <param name="noCache">
        ///     a value indicating whether the track should be returned from cache, if it is cached.
        ///     Note this parameter does only take any effect is a cache provider is specified in constructor.
        /// </param>
        /// <returns>
        ///     a task that represents the asynchronous operation <param>the request response</param>
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the cluster has not been initialized.
        /// </exception>
        public Task<TrackLoadResponsePayload> LoadTracksAsync(string query, SearchMode mode = SearchMode.None, bool noCache = false)
            => GetPreferredNode().LoadTracksAsync(query, mode, noCache);

        /// <summary>
        ///     Creates a new lavalink cluster node.
        /// </summary>
        /// <param name="nodeOptions">the node options</param>
        /// <returns>the created node</returns>
        private LavalinkClusterNode CreateNode(LavalinkNodeOptions nodeOptions)
            => new LavalinkClusterNode(this, nodeOptions, _client, _logger, _cache, _nodeId++);
    }
}