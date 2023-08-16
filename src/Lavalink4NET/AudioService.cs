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
    private readonly AudioServiceOptions _options;
    private readonly ILoggerFactory _loggerFactory;
    private readonly LavalinkNodeServiceContext _serviceContext;
    private LavalinkNode? _node;
    private int _disposeState;

    public AudioService(
        IDiscordClientWrapper discordClient,
        ILavalinkApiClientProvider apiClientProvider,
        IPlayerManager playerManager,
        ITrackManager trackManager,
        ILavalinkSocketFactory socketFactory,
        IIntegrationManager integrationManager,
        ILoggerFactory loggerFactory,
        IOptions<AudioServiceOptions> options) : base(
            discordClient: discordClient,
            apiClientProvider: apiClientProvider,
            integrations: integrationManager,
            players: playerManager,
            tracks: trackManager,
            logger: loggerFactory.CreateLogger<AudioService>())
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(socketFactory);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Value;
        _loggerFactory = loggerFactory;

        _serviceContext = new LavalinkNodeServiceContext(
            ClientWrapper: discordClient,
            LavalinkSocketFactory: socketFactory,
            IntegrationManager: integrationManager,
            PlayerManager: playerManager,
            NodeListener: this);

        Label = _options.Label ?? "default";
    }

    public string Label { get; }

    protected override async ValueTask RunInternalAsync(ClientInformation clientInformation, CancellationToken cancellationToken = default)
    {
        _node = new LavalinkNode(
            serviceContext: _serviceContext,
            options: Options.Create(_options),
            apiEndpoints: new LavalinkApiEndpoints(_options.BaseAddress),
            logger: _loggerFactory.CreateLogger<LavalinkNode>());

        await _node.RunAsync(clientInformation, cancellationToken).ConfigureAwait(false);
    }

    public string? SessionId
    {
        get
        {
            ThrowIfDisposed();
            return _node?.SessionId;
        }
    }

    public override ValueTask WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        if (_node is null)
        {
            throw new InvalidOperationException("The node is not initialized.");
        }

        return _node.WaitForReadyAsync(cancellationToken);
    }

    private void ThrowIfDisposed()
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposeState is not 0, this);
#else
        if (_disposeState is not 0)
        {
            throw new ObjectDisposedException(nameof(AudioService));
        }
#endif
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        if (Interlocked.CompareExchange(ref _disposeState, 1, 0) is 0)
        {
            return;
        }

        await base.DisposeAsyncCore();

        if (_node is not null)
        {
            await _node.DisposeAsync().ConfigureAwait(false);
        }
    }
}