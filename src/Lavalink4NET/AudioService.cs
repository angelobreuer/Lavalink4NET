namespace Lavalink4NET;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Integrations;
using Lavalink4NET.Players;
using Lavalink4NET.Rest;
using Lavalink4NET.Socket;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public partial class AudioService : AudioServiceBase
{
    private readonly CancellationTokenSource _stoppingCancellationTokenSource;
    private readonly AudioServiceOptions _options;
    private readonly ILoggerFactory _loggerFactory;
    private readonly LavalinkNodeServiceContext _serviceContext;
    private LavalinkNode? _node;
    private Task? _executeTask;

    public AudioService(
        IDiscordClientWrapper discordClient,
        ILavalinkApiClientProvider apiClientProvider,
        IPlayerManager playerManager,
        ITrackManager trackManager,
        ILavalinkSocketFactory socketFactory,
        IIntegrationManager integrationManager,
        ILoggerFactory loggerFactory,
        IOptions<AudioServiceOptions> options)
        : base(apiClientProvider, integrationManager, playerManager, trackManager)
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(socketFactory);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(options);

        _stoppingCancellationTokenSource = new CancellationTokenSource();
        _options = options.Value;
        _loggerFactory = loggerFactory;

        _serviceContext = new LavalinkNodeServiceContext(
            ClientWrapper: discordClient,
            LavalinkSocketFactory: socketFactory,
            IntegrationManager: integrationManager,
            PlayerManager: playerManager,
            NodeListener: this);
    }

    public override ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        if (_executeTask is not null)
        {
            return ValueTask.CompletedTask;
        }

        _node = new LavalinkNode(
            serviceContext: _serviceContext,
            options: Options.Create(_options),
            apiEndpoints: new LavalinkApiEndpoints(_options.BaseAddress),
            logger: _loggerFactory.CreateLogger<LavalinkNode>());

        _executeTask = RunInternalAsync(_node, cancellationToken);

        return _executeTask.IsCompleted ? new ValueTask(_executeTask) : ValueTask.CompletedTask;
    }

    private async Task RunInternalAsync(LavalinkNode node, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            token1: cancellationToken,
            token2: _stoppingCancellationTokenSource.Token);

        await node.RunAsync(cancellationTokenSource.Token).ConfigureAwait(false);
    }

    public override async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_executeTask is null)
        {
            return;
        }

        try
        {
            _stoppingCancellationTokenSource.Cancel();
        }
        finally
        {
            await _executeTask
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    public string? SessionId => _node?.SessionId;

    public override ValueTask WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_node is null)
        {
            throw new InvalidOperationException("The node is not initialized.");
        }

        return _node.WaitForReadyAsync(cancellationToken);
    }

    public override void Dispose()
    {
        // TODO
    }
}