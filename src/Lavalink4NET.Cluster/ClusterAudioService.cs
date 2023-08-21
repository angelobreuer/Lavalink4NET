namespace Lavalink4NET.Cluster;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Cluster.Nodes;
using Lavalink4NET.Integrations;
using Lavalink4NET.Players;
using Lavalink4NET.Rest;
using Lavalink4NET.Socket;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class ClusterAudioService : AudioServiceBase, IClusterAudioService
{
	private readonly object _syncRoot;
	private readonly ClusterAudioServiceOptions _options;
	private readonly LavalinkNodeServiceContext _serviceContext;
	private readonly ILavalinkApiClientFactory _lavalinkApiClientFactory;
	private readonly ILoggerFactory _loggerFactory;
	private readonly TaskCompletionSource<ClientInformation> _clientReadyTaskCompletionSource;

	public ClusterAudioService(
		IDiscordClientWrapper discordClient,
		ILavalinkSocketFactory socketFactory,
		ILavalinkApiClientProvider lavalinkApiClientProvider,
		ILavalinkApiClientFactory lavalinkApiClientFactory,
		IIntegrationManager integrations,
		IPlayerManager players,
		ITrackManager tracks,
		IOptions<ClusterAudioServiceOptions> options,
		ILoggerFactory loggerFactory)
		: base(discordClient, lavalinkApiClientProvider, integrations, players, tracks, options.Value.ReadyTimeout, loggerFactory.CreateLogger<ClusterAudioService>())
	{
		ArgumentNullException.ThrowIfNull(options);
		ArgumentNullException.ThrowIfNull(discordClient);
		ArgumentNullException.ThrowIfNull(loggerFactory);

		_clientReadyTaskCompletionSource = new TaskCompletionSource<ClientInformation>(
			creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);

		_syncRoot = new object();
		_options = options.Value;

		_serviceContext = new LavalinkNodeServiceContext(
			ClientWrapper: discordClient,
			LavalinkSocketFactory: socketFactory,
			IntegrationManager: Integrations,
			PlayerManager: Players,
			NodeListener: this);

		Nodes = ImmutableArray<ILavalinkNode>.Empty;
		_lavalinkApiClientFactory = lavalinkApiClientFactory;
		_loggerFactory = loggerFactory;
	}

	public ImmutableArray<ILavalinkNode> Nodes { get; private set; }

	public ValueTask<ILavalinkNode> AddAsync(IOptions<LavalinkClusterNodeOptions> options, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ArgumentNullException.ThrowIfNull(options);

		var nodeLogger = _loggerFactory.CreateLogger<LavalinkNode>();
		var apiClient = _lavalinkApiClientFactory.Create(options);

		var node = new LavalinkClusterNode(
			cluster: this,
			apiClient: apiClient,
			serviceContext: _serviceContext,
			options: options,
			shutdownCancellationToken: ShutdownCancellationToken,
			readyTask: _clientReadyTaskCompletionSource.Task,
			logger: nodeLogger);

		lock (_syncRoot)
		{
			Nodes = Nodes.Add(node);
		}

		return ValueTask.FromResult<ILavalinkNode>(node);
	}

	public async ValueTask<bool> RemoveAsync(ILavalinkNode node, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(node);

		lock (_syncRoot)
		{
			var previousNodes = Nodes;
			Nodes = previousNodes.Remove(node);

			if (previousNodes == Nodes)
			{
				return false;
			}
		}

		await node
			.StopAsync(cancellationToken)
			.ConfigureAwait(false);

		return true;
	}

	protected override async ValueTask RunInternalAsync(ClientInformation clientInformation, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		_clientReadyTaskCompletionSource.TrySetResult(clientInformation);

		foreach (var nodeOptions in _options.Nodes)
		{
			await AddAsync(Options.Create(nodeOptions), cancellationToken).ConfigureAwait(false);
		}

		try
		{
			await Task
				.Delay(Timeout.Infinite, cancellationToken)
				.ConfigureAwait(false);
		}
		catch (OperationCanceledException)
		{
			// ignore
		}

		foreach (var node in Nodes)
		{
			await RemoveAsync(node, cancellationToken).ConfigureAwait(false);
		}
	}
}
