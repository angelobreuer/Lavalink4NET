namespace Lavalink4NET.Players;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Integrations;
using Lavalink4NET.Protocol.Payloads;
using Lavalink4NET.Rest;
using Lavalink4NET.Socket;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed partial class AudioService : IAudioService
{
    private readonly TaskCompletionSource _readyTaskCompletionSource;
    private readonly ILogger<AudioService> _logger;
    private readonly IDiscordClientWrapper _clientWrapper;
    private readonly ILoggerFactory _loggerFactory;
    private readonly LavalinkNodeOptions _options;

    public AudioService(
        IDiscordClientWrapper clientWrapper,
        ILavalinkApiClient apiClient,
        ITrackManager trackManager,
        ILoggerFactory loggerFactory,
        IOptions<LavalinkNodeOptions> options)
    {
        ArgumentNullException.ThrowIfNull(clientWrapper);
        ArgumentNullException.ThrowIfNull(apiClient);
        ArgumentNullException.ThrowIfNull(trackManager);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(options);

        Players = new PlayerManager(clientWrapper, apiClient);
        Tracks = trackManager;
        Integrations = new IntegrationCollection();

        _clientWrapper = clientWrapper;
        _loggerFactory = loggerFactory;
        _options = options.Value;
        _logger = loggerFactory.CreateLogger<AudioService>();

        _readyTaskCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        _ = ReceiveAsync(); // TODO
    }

    public string? SessionId { get; private set; }

    public bool IsReady => _readyTaskCompletionSource.Task.IsCompletedSuccessfully;

    public IPlayerManager Players { get; }

    public IIntegrationCollection Integrations { get; }

    public ITrackManager Tracks { get; }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public ValueTask WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var task = _readyTaskCompletionSource.Task;

        if (cancellationToken.IsCancellationRequested)
        {
            task = task.WaitAsync(cancellationToken);
        }

        return new ValueTask(task);
    }

    private async ValueTask ProcessEventAsync(IEventPayload payload, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(payload);

    }

    private async ValueTask ProcessPayloadAsync(IPayload payload, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(payload);


        if (payload is IEventPayload eventPayload)
        {
            await ProcessEventAsync(eventPayload, cancellationToken).ConfigureAwait(false);
            return;
        }

        if (payload is ReadyPayload readyPayload)
        {
            if (!_readyTaskCompletionSource.TrySetResult())
            {
                _logger.LogWarning("Multiple ready payloads were received.");
            }

            SessionId = readyPayload.SessionId;

            _logger.LogInformation("Lavalink4NET is ready (session identifier: {SessionId}).", SessionId);
        }

        if (SessionId is null)
        {
            _logger.LogWarning("A payload was received before the ready payload was received. The payload will be ignored.");
            return;
        }

        if (payload is PlayerUpdatePayload playerUpdatePayload)
        {

        }

        if (payload is StatisticsPayload statisticsPayload)
        {

        }
    }

    private async Task ReceiveAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        static async ValueTask<ClientInformation?> WaitForClientReadyAsync(IDiscordClientWrapper clientWrapper, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(clientWrapper);

            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));

            try
            {
                return await clientWrapper
                    .WaitForReadyAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return null;
            }
        }

        _logger.LogDebug("Waiting for client being ready...");
        _logger.LogInformation("Lavalink node initialized.");

        var clientInformation = await WaitForClientReadyAsync(
            clientWrapper: _clientWrapper,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (clientInformation is null)
        {
            var exception = new TimeoutException("Timed out while waiting for discord client being ready.");
            _logger.LogError(exception, "Timed out while waiting for discord client being ready.");
            throw exception;
        }

        _logger.LogDebug("Discord client ({ClientLabel}) is ready.", clientInformation.Value.Label);

        var socketOptions = new LavalinkSocketOptions
        {
            HttpClientName = _options.HttpClientName,
            Uri = _options.Uri,
            ShardCount = clientInformation.Value.ShardCount,
            UserId = clientInformation.Value.CurrentUserId,
        };

        using var socket = new LavalinkSocket(
            logger: _loggerFactory.CreateLogger<LavalinkSocket>(),
            options: Options.Create(socketOptions));

        while (true)
        {
            var payload = await socket
                .ReceiveAsync(cancellationToken)
                .ConfigureAwait(false);

            await ProcessPayloadAsync(payload, cancellationToken).ConfigureAwait(false);

            foreach (var (_, integration) in Integrations)
            {
                // TODO: error handling
                await integration
                    .ProcessPayloadAsync(payload, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
