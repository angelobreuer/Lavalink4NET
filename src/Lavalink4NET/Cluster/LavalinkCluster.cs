/*
 *  File:   LavalinkCluster.cs
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Events;
using Lavalink4NET.Integrations;
using Lavalink4NET.Player;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Rest;

/// <summary>
///     A set of lavalink nodes bound to a cluster usable for voice node balancing.
/// </summary>
public class LavalinkCluster : IAudioService, ILavalinkRestClient, IDisposable
{
    private readonly IMemoryCache? _cache;
    private readonly IDiscordClientWrapper _client;
    private readonly LoadBalancingStrategy _loadBalacingStrategy;
    private readonly ILogger<LavalinkCluster>? _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly List<LavalinkClusterNode> _nodes;
    private readonly object _nodesLock;
    private bool _disposed;
    private bool _initialized;
    private volatile int _nodeId;
    private readonly ClusterNodeFactory _nodeFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LavalinkCluster"/> class.
    /// </summary>
    /// <param name="options">the cluster options</param>
    /// <param name="client">the discord client</param>
    /// <param name="loggerFactory">the logger</param>
    /// <param name="cache">
    ///     a cache that is shared between the different lavalink rest clients. If the cache is
    ///     <see langword="null"/>, no cache will be used.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="options"/> parameter is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="client"/> is <see langword="null"/>.
    /// </exception>
    public LavalinkCluster(LavalinkClusterOptions options, IDiscordClientWrapper client, ILoggerFactory? loggerFactory = null, IMemoryCache? cache = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(client);

        _client = client;

        _loadBalacingStrategy = options.LoadBalacingStrategy;
        _logger = loggerFactory?.CreateLogger<LavalinkCluster>();
        _loggerFactory = loggerFactory;
        _cache = cache;
        _nodesLock = new object();
        StayOnline = options.StayOnline;
        _nodeFactory = options.NodeFactory;
        _nodes = options.Nodes.Select(CreateNode).ToList();

        Integrations = new IntegrationCollection();
    }

    /// <inheritdoc/>
    public event AsyncEventHandler<NodeConnectedEventArgs>? NodeConnected;

    /// <inheritdoc/>
    public event AsyncEventHandler<NodeDisconnectedEventArgs>? NodeDisconnected;

    /// <inheritdoc/>
    public event AsyncEventHandler<PlayerMovedEventArgs>? PlayerMoved;

    /// <inheritdoc/>
    public event AsyncEventHandler<TrackEndEventArgs>? TrackEnd;

    /// <inheritdoc/>
    public event AsyncEventHandler<TrackExceptionEventArgs>? TrackException;

    /// <inheritdoc/>
    public event AsyncEventHandler<TrackStartedEventArgs>? TrackStarted;

    /// <inheritdoc/>
    public event AsyncEventHandler<TrackStuckEventArgs>? TrackStuck;

    /// <summary>
    ///     Gets all nodes.
    /// </summary>
    public IReadOnlyList<LavalinkClusterNode> Nodes
    {
        get
        {
            lock (_nodesLock)
            {
                return _nodes.ToArray();
            }
        }
    }

    /// <summary>
    ///     Gets the preferred node using the <see cref="LoadBalancingStrategy"/> specified in
    ///     the options.
    /// </summary>
    /// <returns>the next preferred node</returns>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the cluster has not been initialized.
    /// </exception>
    /// <exception cref="InvalidOperationException">thrown if no nodes is available</exception>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public LavalinkNode PreferredNode => GetPreferredNode();

    /// <summary>
    ///     Gets or sets a value indicating whether stay-online should be enabled for the cluster.
    /// </summary>
    /// <remarks>
    ///     When this option is enabled, the cluster will try to move the players of a
    ///     disconnected node to a new.
    /// </remarks>
    public bool StayOnline { get; set; }

    public IIntegrationCollection Integrations { get; }

    /// <summary>
    ///     Dynamically adds a node to the cluster asynchronously.
    /// </summary>
    /// <param name="nodeOptions">the node connection options</param>
    /// <returns>
    ///     a task that represents the asynchronous operation
    ///     <para>the cluster node info created for the node</para>
    /// </returns>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public async Task<LavalinkClusterNode> AddNodeAsync(LavalinkNodeOptions nodeOptions)
    {
        EnsureNotDisposed();

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
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        lock (_nodesLock)
        {
            _nodes.ForEach(s => s.Dispose());
            _nodes.Clear();
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public LavalinkPlayer? GetPlayer(ulong guildId) => GetPlayer<LavalinkPlayer>(guildId);

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public TPlayer? GetPlayer<TPlayer>(ulong guildId) where TPlayer : LavalinkPlayer
        => GetServingNode(guildId, NodeRequestType.PlayTrack).GetPlayer<TPlayer>(guildId);

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public IReadOnlyList<TPlayer> GetPlayers<TPlayer>() where TPlayer : LavalinkPlayer
    {
        EnsureNotDisposed();

        lock (_nodesLock)
        {
            return _nodes
                .SelectMany(s => s.GetPlayers<TPlayer>())
                .ToList();
        }
    }

    /// <summary>
    ///     Gets the preferred node using the <see cref="LoadBalancingStrategy"/> specified in
    ///     the options.
    /// </summary>
    /// <param name="type">the type of the purpose for the node</param>
    /// <returns>the next preferred node</returns>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the cluster has not been initialized.
    /// </exception>
    /// <exception cref="InvalidOperationException">thrown if no nodes is available</exception>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public LavalinkNode GetPreferredNode(NodeRequestType type = NodeRequestType.Unspecified)
    {
        EnsureNotDisposed();

        if (!_initialized)
        {
            throw new InvalidOperationException("The cluster has not been initialized.");
        }

        lock (_nodesLock)
        {
            return GetPreferredNodeInternal(type);
        }
    }

    /// <summary>
    ///     Gets the node that serves the guild specified by <paramref name="guildId"/> (if no
    ///     node serves the guild, <see cref="PreferredNode"/> is used).
    /// </summary>
    /// <param name="guildId">the guild snowflake identifier</param>
    /// <param name="type">the type of the purpose for the node</param>
    /// <returns>the serving node for the specified <paramref name="guildId"/></returns>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the cluster has not been initialized.
    /// </exception>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public LavalinkNode GetServingNode(ulong guildId, NodeRequestType type = NodeRequestType.Unspecified)
    {
        EnsureNotDisposed();

        lock (_nodesLock)
        {
            var node = _nodes.FirstOrDefault(s => s.HasPlayer(guildId));

            if (node != null)
            {
                return node;
            }

            return GetPreferredNodeInternal(type);
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public ValueTask<LavalinkTrack?> GetTrackAsync(string query, SearchMode mode = SearchMode.None,
        bool noCache = false, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return GetPreferredNode(NodeRequestType.LoadTrack).GetTrackAsync(query, mode, noCache, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public ValueTask<IEnumerable<LavalinkTrack>> GetTracksAsync(
        string query,
        SearchMode mode = SearchMode.None,
        bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return GetPreferredNode(NodeRequestType.LoadTrack).GetTracksAsync(query, mode, noCache, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public bool HasPlayer(ulong guildId)
    {
        EnsureNotDisposed();

        lock (_nodesLock)
        {
            return _nodes.Any(s => s.HasPlayer(guildId));
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public async Task InitializeAsync()
    {
        EnsureNotDisposed();

        if (_initialized)
        {
            return;
        }

        await Task.WhenAll(_nodes.Select(s => s.InitializeAsync()));

        _initialized = true;
    }

    /// <inheritdoc/>
    public Task<TPlayer> JoinAsync<TPlayer>(PlayerFactory<TPlayer> playerFactory, ulong guildId, ulong voiceChannelId, bool selfDeaf = false, bool selfMute = false) where TPlayer : LavalinkPlayer
        => GetServingNode(guildId, NodeRequestType.PlayTrack).JoinAsync(playerFactory, guildId, voiceChannelId, selfDeaf, selfMute);

    /// <inheritdoc/>
    public Task<TPlayer> JoinAsync<TPlayer>(ulong guildId, ulong voiceChannelId, bool selfDeaf = false, bool selfMute = false) where TPlayer : LavalinkPlayer, new()
        => GetServingNode(guildId, NodeRequestType.PlayTrack).JoinAsync<TPlayer>(guildId, voiceChannelId, selfDeaf, selfMute);

    /// <inheritdoc/>
    public Task<LavalinkPlayer> JoinAsync(ulong guildId, ulong voiceChannelId, bool selfDeaf = false, bool selfMute = false)
        => GetServingNode(guildId, NodeRequestType.PlayTrack).JoinAsync(guildId, voiceChannelId, selfDeaf, selfMute);

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public ValueTask<TrackLoadResponsePayload> LoadTracksAsync(
        string query,
        SearchMode mode = SearchMode.None,
        bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return GetPreferredNode(NodeRequestType.LoadTrack).LoadTracksAsync(query, mode, noCache, cancellationToken);
    }

    /// <summary>
    ///     An internal callback when a cluster node connected to the cluster asynchronously.
    /// </summary>
    /// <param name="node">the node where the connection was opened</param>
    /// <param name="eventArgs">the event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    internal Task NodeConnectedAsync(LavalinkClusterNode node, ConnectedEventArgs eventArgs)
        => OnNodeConnectedAsync(new NodeConnectedEventArgs(node, eventArgs));

    /// <summary>
    ///     An internal callback when a cluster node disconnected from the cluster asynchronously.
    /// </summary>
    /// <param name="node">the node where the connection was closed</param>
    /// <param name="eventArgs">the event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    internal Task NodeDisconnectedAsync(LavalinkClusterNode node, DisconnectedEventArgs eventArgs)
        => OnNodeDisconnectedAsync(new NodeDisconnectedEventArgs(node, eventArgs));

    internal Task OnTrackEndAsync(TrackEndEventArgs eventArgs)
        => TrackEnd.InvokeAsync(this, eventArgs);

    internal Task OnTrackExceptionAsync(TrackExceptionEventArgs eventArgs)
        => TrackException.InvokeAsync(this, eventArgs);

    internal Task OnTrackStartedAsync(TrackStartedEventArgs eventArgs)
        => TrackStarted.InvokeAsync(this, eventArgs);

    internal Task OnTrackStuckAsync(TrackStuckEventArgs eventArgs)
        => TrackStuck.InvokeAsync(this, eventArgs);

    /// <summary>
    ///     Triggers the <see cref="NodeConnected"/> event asynchronously.
    /// </summary>
    /// <param name="eventArgs">the event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    protected virtual Task OnNodeConnectedAsync(NodeConnectedEventArgs eventArgs)
        => NodeConnected.InvokeAsync(this, eventArgs);

    /// <summary>
    ///     Triggers the <see cref="NodeDisconnected"/> event asynchronously.
    /// </summary>
    /// <param name="eventArgs">the event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    protected virtual async Task OnNodeDisconnectedAsync(NodeDisconnectedEventArgs eventArgs)
    {
        // stay-online feature
        if (StayOnline && eventArgs.ByRemote)
        {
            _logger?.LogWarning("(Stay-Online) Node died! Moving players to a new node...");

            var players = eventArgs.Node.GetPlayers<LavalinkPlayer>();

            // move all players
            var tasks = players.Select(player => MovePlayerToNewNodeAsync(eventArgs.Node, player)).ToArray();

            // await until all players were moved to a new node
            await Task.WhenAll(tasks);
        }

        // invoke event
        await NodeDisconnected.InvokeAsync(this, eventArgs);
    }

    /// <summary>
    ///     Dispatches the <see cref="PlayerMoved"/> event asynchronously.
    /// </summary>
    /// <param name="eventArgs">the event arguments passed with the event</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    protected virtual Task OnPlayerMovedAsync(PlayerMovedEventArgs eventArgs)
        => PlayerMoved.InvokeAsync(this, eventArgs);

    /// <summary>
    ///     Creates a new lavalink cluster node.
    /// </summary>
    /// <param name="nodeOptions">the node options</param>
    /// <returns>the created node</returns>
    private LavalinkClusterNode CreateNode(LavalinkNodeOptions nodeOptions) => _nodeFactory(
        cluster: this,
        options: nodeOptions,
        client: _client,
        id: _nodeId++,
        integrationCollection: Integrations,
        logger: _loggerFactory.CreateLogger<LavalinkClusterNode>(),
        cache: _cache);

    /// <summary>
    ///     Throws an exception if the <see cref="LavalinkCluster"/> instance is disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    private void EnsureNotDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(LavalinkCluster));
        }
    }

    private LavalinkNode GetPreferredNodeInternal(NodeRequestType type = NodeRequestType.Unspecified)
    {
        // find a connected node
        var nodes = _nodes.Where(s => s.IsConnected).ToArray();

        // no nodes available
        if (nodes.Length == 0)
        {
            throw new InvalidOperationException("No node available.");
        }

        // get the preferred node by the load balancing strategy
        var node = _loadBalacingStrategy(this, nodes, type);

        // update last usage
        node.LastUsage = DateTimeOffset.UtcNow;

        return node;
    }

    private async Task MovePlayerToNewNodeAsync(LavalinkNode sourceNode, LavalinkPlayer player)
    {
        if (player is null)
        {
            throw new ArgumentNullException(nameof(player));
        }

        if (!Nodes.Any(s => s.IsConnected))
        {
            _logger?.LogInformation($"(Stay-Online) No node available for player {player.GuildId}, dropping player...");

            // invoke event
            await OnPlayerMovedAsync(new PlayerMovedEventArgs(sourceNode, null, player));
            return;
        }

        // move node
        var targetNode = GetPreferredNode(NodeRequestType.Backup);
        await sourceNode.MovePlayerAsync(player, targetNode);

        // invoke event
        await OnPlayerMovedAsync(new PlayerMovedEventArgs(sourceNode, targetNode, player));
    }
}
