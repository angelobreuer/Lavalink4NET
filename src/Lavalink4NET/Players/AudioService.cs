namespace Lavalink4NET.Players;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Integrations;
using Lavalink4NET.Protocol.Payloads;
using Lavalink4NET.Rest;
using Lavalink4NET.Socket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed partial class AudioService : IAudioService
{
    private readonly TaskCompletionSource _readyTaskCompletionSource;
    private readonly LavalinkSocket _socket;
    private readonly ILogger<AudioService> _logger;

    public AudioService(IDiscordClientWrapper clientWrapper, ILavalinkApiClient apiClient, ILoggerFactory loggerFactory)
    {
        // TODO
        var socketOptions = new LavalinkSocketOptions
        {

        };

        Players = new PlayerManager(clientWrapper, apiClient);
        Integrations = new IntegrationCollection();

        _logger = loggerFactory.CreateLogger<AudioService>();

        _socket = new LavalinkSocket(
            logger: loggerFactory.CreateLogger<LavalinkSocket>(),
            options: Options.Create(socketOptions));

        _readyTaskCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        _ = ReceiveAsync(); // TODO
    }

    public string? SessionId { get; private set; }

    public bool IsReady => _readyTaskCompletionSource.Task.IsCompletedSuccessfully;

    public IPlayerManager Players { get; }

    public IIntegrationCollection Integrations { get; }

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

        while (true)
        {
            var payload = await _socket
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
