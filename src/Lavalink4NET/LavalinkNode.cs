namespace Lavalink4NET;

using System;
using System.Buffers;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Events;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players;
using Lavalink4NET.Protocol;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Protocol.Payloads;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Protocol.Requests;
using Lavalink4NET.Rest;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Rest.Entities.Usage;
using Lavalink4NET.Socket;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class LavalinkNode : IAsyncDisposable
{
    private readonly CancellationTokenSource _shutdownCancellationTokenSource;
    private readonly CancellationToken _shutdownCancellationToken;
    private readonly LavalinkNodeOptions _options;
    private readonly LavalinkNodeServiceContext _serviceContext;
    private readonly ILavalinkApiClient _apiClient;
    private readonly LavalinkApiEndpoints _apiEndpoints;
    private readonly ILogger<LavalinkNode> _logger;
    private readonly Stopwatch _readyStopwatch;
    private TaskCompletionSource<string> _readyTaskCompletionSource;
    private Task? _executeTask;
    private bool _disposed;

    public LavalinkNode(
        LavalinkNodeServiceContext serviceContext,
        ILavalinkApiClient apiClient,
        IOptions<LavalinkNodeOptions> options,
        LavalinkApiEndpoints apiEndpoints,
        ILogger<LavalinkNode> logger)
    {
        ArgumentNullException.ThrowIfNull(serviceContext);
        ArgumentNullException.ThrowIfNull(apiClient);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _readyTaskCompletionSource = new TaskCompletionSource<string>(
            creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);

        _serviceContext = serviceContext;
        _apiClient = apiClient;
        _apiEndpoints = apiEndpoints;
        _options = options.Value;
        _logger = logger;

        _readyStopwatch = new Stopwatch();

        Label = _options.Label ?? $"Lavalink-{CorrelationIdGenerator.GetNextId()}";

        _shutdownCancellationTokenSource = new CancellationTokenSource();
        _shutdownCancellationToken = _shutdownCancellationTokenSource.Token;
    }

    public string Label { get; }

    public bool IsReady => _readyTaskCompletionSource.Task.IsCompletedSuccessfully;

    public string? SessionId { get; private set; }

    public async ValueTask WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        await _readyTaskCompletionSource.Task
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private static LavalinkTrack CreateTrack(TrackModel track) => new()
    {
        Duration = track.Information.Duration,
        Identifier = track.Information.Identifier,
        IsLiveStream = track.Information.IsLiveStream,
        IsSeekable = track.Information.IsSeekable,
        SourceName = track.Information.SourceName,
        StartPosition = track.Information.Position,
        Title = track.Information.Title,
        Uri = track.Information.Uri,
        TrackData = track.Data,
        Author = track.Information.Author,
        ArtworkUri = track.Information.ArtworkUri,
        Isrc = track.Information.Isrc,
        AdditionalInformation = track.AdditionalInformation,
    };

    private static string SerializePayload(IPayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        var jsonWriterOptions = new JsonWriterOptions
        {
            Indented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        var arrayBufferWriter = new ArrayBufferWriter<byte>();
        var utf8JsonWriter = new Utf8JsonWriter(arrayBufferWriter, jsonWriterOptions);

        JsonSerializer.Serialize(utf8JsonWriter, payload, ProtocolSerializerContext.Default.IPayload);

        return Encoding.UTF8.GetString(arrayBufferWriter.WrittenSpan);
    }

    private async ValueTask ProcessEventAsync(IEventPayload payload, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(payload);

        var player = await _serviceContext.PlayerManager
            .GetPlayerAsync(payload.GuildId, cancellationToken)
            .ConfigureAwait(false);

        if (player is null)
        {
            _logger.ReceivedEventPayloadForNonRegisteredPlayer(Label, payload.GuildId);
            return;
        }

        var task = payload switch
        {
            TrackEndEventPayload trackEvent => ProcessTrackEndEventAsync(player, trackEvent, cancellationToken),
            TrackStartEventPayload trackEvent => ProcessTrackStartEventAsync(player, trackEvent, cancellationToken),
            TrackStuckEventPayload trackEvent => ProcessTrackStuckEventAsync(player, trackEvent, cancellationToken),
            TrackExceptionEventPayload trackEvent => ProcessTrackExceptionEventAsync(player, trackEvent, cancellationToken),
            WebSocketClosedEventPayload closedEvent => ProcessWebSocketClosedEventAsync(player, closedEvent, cancellationToken),
            _ => ValueTask.CompletedTask,
        };

        await task.ConfigureAwait(false);
    }

    private async ValueTask ProcessPayloadAsync(IPayload payload, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(payload);

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.ReceivedPayloadFromLavalinkNode(Label, SerializePayload(payload));
        }

        if (payload is ReadyPayload readyPayload)
        {
            if (!_readyTaskCompletionSource.TrySetResult(readyPayload.SessionId))
            {
                _logger.MultipleReadyPayloadsReceived(Label);
            }

            SessionId = readyPayload.SessionId;

            // Enable resuming, if wanted
            if (_options.ResumptionOptions.IsEnabled && !readyPayload.SessionResumed)
            {
                var sessionUpdateProperties = new SessionUpdateProperties
                {
                    IsSessionResumptionEnabled = true,
                    Timeout = _options.ResumptionOptions.Timeout.Value,
                };

                await _apiClient
                    .UpdateSessionAsync(readyPayload.SessionId, sessionUpdateProperties, cancellationToken)
                    .ConfigureAwait(false);
            }

            _logger.Ready(Label, SessionId);
        }

        if (SessionId is null)
        {
            _logger.PayloadReceivedBeforeReadyPayload(Label);
            return;
        }

        if (payload is IEventPayload eventPayload)
        {
            await ProcessEventAsync(eventPayload, cancellationToken).ConfigureAwait(false);
            return;
        }

        if (payload is PlayerUpdatePayload playerUpdatePayload)
        {
            var player = await _serviceContext.PlayerManager
                .GetPlayerAsync(playerUpdatePayload.GuildId, cancellationToken)
                .ConfigureAwait(false);

            if (player is null)
            {
                _logger.ReceivedPlayerUpdatePayloadForNonRegisteredPlayer(Label, playerUpdatePayload.GuildId);
                return;
            }

            if (player is ILavalinkPlayerListener playerListener)
            {
                var state = playerUpdatePayload.State;

                await playerListener
                    .NotifyPlayerUpdateAsync(state.AbsoluteTimestamp, state.Position, state.IsConnected, state.Latency, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        if (payload is StatisticsPayload statisticsPayload)
        {
            await ProcessStatisticsPayloadAsync(statisticsPayload, cancellationToken).ConfigureAwait(false);
        }
    }

    private async ValueTask ProcessStatisticsPayloadAsync(StatisticsPayload statisticsPayload, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(statisticsPayload);

        var memoryUsage = new ServerMemoryUsageStatistics(
            FreeMemory: statisticsPayload.MemoryUsage.FreeMemory,
            UsedMemory: statisticsPayload.MemoryUsage.UsedMemory,
            AllocatedMemory: statisticsPayload.MemoryUsage.AllocatedMemory,
            ReservableMemory: statisticsPayload.MemoryUsage.ReservableMemory);

        var processorUsage = new ServerProcessorUsageStatistics(
            CoreCount: statisticsPayload.ProcessorUsage.CoreCount,
            SystemLoad: statisticsPayload.ProcessorUsage.SystemLoad,
            LavalinkLoad: statisticsPayload.ProcessorUsage.LavalinkLoad);

        var frameStatistics = statisticsPayload.FrameStatistics is null ? default(ServerFrameStatistics?) : new ServerFrameStatistics(
            SentFrames: statisticsPayload.FrameStatistics.SentFrames,
            NulledFrames: statisticsPayload.FrameStatistics.NulledFrames,
            DeficitFrames: statisticsPayload.FrameStatistics.DeficitFrames);

        var statistics = new LavalinkServerStatistics(
            ConnectedPlayers: statisticsPayload.ConnectedPlayers,
            PlayingPlayers: statisticsPayload.PlayingPlayers,
            Uptime: statisticsPayload.Uptime,
            MemoryUsage: memoryUsage,
            ProcessorUsage: processorUsage,
            FrameStatistics: frameStatistics);

        var eventArgs = new StatisticsUpdatedEventArgs(statistics);

        await _serviceContext.NodeListener
            .OnStatisticsUpdatedAsync(eventArgs, cancellationToken)
            .ConfigureAwait(false);
    }

    private async ValueTask ProcessTrackEndEventAsync(ILavalinkPlayer player, TrackEndEventPayload trackEndEvent, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(trackEndEvent);

        var track = CreateTrack(trackEndEvent.Track);

        if (player is ILavalinkPlayerListener playerListener)
        {
            await playerListener
                .NotifyTrackEndedAsync(track, trackEndEvent.Reason, cancellationToken)
                .ConfigureAwait(false);
        }

        var eventArgs = new TrackEndedEventArgs(
            player: player,
            track: track,
            reason: trackEndEvent.Reason);

        await _serviceContext.NodeListener
            .OnTrackEndedAsync(eventArgs, cancellationToken)
            .ConfigureAwait(false);
    }

    private async ValueTask ProcessTrackExceptionEventAsync(ILavalinkPlayer player, TrackExceptionEventPayload trackExceptionEvent, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(trackExceptionEvent);

        var track = CreateTrack(trackExceptionEvent.Track);

        var exception = new TrackException(
            Severity: trackExceptionEvent.Exception.Severity,
            Message: trackExceptionEvent.Exception.Message,
            Cause: trackExceptionEvent.Exception.Cause);

        if (player is ILavalinkPlayerListener playerListener)
        {
            await playerListener
                .NotifyTrackExceptionAsync(track, exception, cancellationToken)
                .ConfigureAwait(false);
        }

        var eventArgs = new TrackExceptionEventArgs(
            player: player,
            track: track,
            exception: exception);

        await _serviceContext.NodeListener
            .OnTrackExceptionAsync(eventArgs, cancellationToken)
            .ConfigureAwait(false);
    }

    private async ValueTask ProcessTrackStartEventAsync(ILavalinkPlayer player, TrackStartEventPayload trackStartEvent, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(trackStartEvent);

        var track = CreateTrack(trackStartEvent.Track);

        if (player is ILavalinkPlayerListener playerListener)
        {
            await playerListener
                .NotifyTrackStartedAsync(track, cancellationToken)
                .ConfigureAwait(false);
        }

        var eventArgs = new TrackStartedEventArgs(
            player: player,
            track: track);

        await _serviceContext.NodeListener
            .OnTrackStartedAsync(eventArgs, cancellationToken)
            .ConfigureAwait(false);
    }

    private async ValueTask ProcessTrackStuckEventAsync(ILavalinkPlayer player, TrackStuckEventPayload trackStuckEvent, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(trackStuckEvent);

        var track = CreateTrack(trackStuckEvent.Track);

        if (player is ILavalinkPlayerListener playerListener)
        {
            await playerListener
                .NotifyTrackStuckAsync(track, trackStuckEvent.ExceededThreshold, cancellationToken)
                .ConfigureAwait(false);
        }

        var eventArgs = new TrackStuckEventArgs(
            player: player,
            track: track,
            threshold: trackStuckEvent.ExceededThreshold);

        await _serviceContext.NodeListener
            .OnTrackStuckAsync(eventArgs, cancellationToken)
            .ConfigureAwait(false);
    }

    private async ValueTask ProcessWebSocketClosedEventAsync(ILavalinkPlayer player, WebSocketClosedEventPayload webSocketClosedEvent, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(webSocketClosedEvent);

        var closeCode = (WebSocketCloseStatus)webSocketClosedEvent.Code;

        if (player is ILavalinkPlayerListener playerListener)
        {
            await playerListener
                .NotifyWebSocketClosedAsync(closeCode, webSocketClosedEvent.Reason, webSocketClosedEvent.WasByRemote, cancellationToken)
                .ConfigureAwait(false);
        }

        var eventArgs = new WebSocketClosedEventArgs(
            player: player,
            closeCode: closeCode,
            reason: webSocketClosedEvent.Reason,
            byRemote: webSocketClosedEvent.WasByRemote);

        await _serviceContext.NodeListener
            .OnWebSocketClosedAsync(eventArgs, cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask RunAsync(ClientInformation clientInformation, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        cancellationToken.ThrowIfCancellationRequested();

        if (_executeTask is not null)
        {
            throw new InvalidOperationException("The node was already started.");
        }

        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            token1: cancellationToken,
            token2: _shutdownCancellationToken);

        var linkedCancellationToken = cancellationTokenSource.Token;

        try
        {
            _executeTask = ReceiveInternalAsync(clientInformation, linkedCancellationToken);
            await _executeTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
        catch (Exception exception)
        {
            _readyTaskCompletionSource.TrySetException(exception);
            throw;
        }
        finally
        {
            _readyTaskCompletionSource.TrySetCanceled(CancellationToken.None);
        }
    }

    private async Task ReceiveInternalAsync(ClientInformation clientInformation, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        var webSocketUri = _options.WebSocketUri ?? _apiEndpoints.WebSocket;

        _readyStopwatch.Restart();

        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_shutdownCancellationToken);
        using var __ = new CancellationTokenDisposable(cancellationTokenSource);

        while (!_shutdownCancellationToken.IsCancellationRequested)
        {
            if (_readyTaskCompletionSource.Task.IsCompleted)
            {
                // Initiate reconnect
                _readyTaskCompletionSource = new TaskCompletionSource<string>(
                    creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);
            }

            var socketOptions = new LavalinkSocketOptions
            {
                Label = Label,
                HttpClientName = _options.HttpClientName,
                Uri = webSocketUri,
                ShardCount = clientInformation.ShardCount,
                UserId = clientInformation.CurrentUserId,
                Passphrase = _options.Passphrase,
                SessionId = SessionId,
            };

            using var socket = _serviceContext.LavalinkSocketFactory.Create(Options.Create(socketOptions));
            using var socketCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token);
            using var ___ = new CancellationTokenDisposable(socketCancellationSource);

            if (socket is null)
            {
                break;
            }

            _ = socket.RunAsync(socketCancellationSource.Token).AsTask();

            await ReceiveInternalAsync(socket, cancellationToken).ConfigureAwait(false);
        }
    }

    private async ValueTask ReceiveInternalAsync(ILavalinkSocket socket, CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IPayload? payload;
            try
            {
                payload = await socket
                    .ReceiveAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.ExceptionOccurredDuringCommunication(Label, exception);
                break;
            }

            if (payload is null)
            {
                break;
            }

            await ProcessPayloadAsync(payload, cancellationToken).ConfigureAwait(false);

            foreach (var (_, integration) in _serviceContext.IntegrationManager)
            {
                try
                {
                    await integration
                        .ProcessPayloadAsync(payload, cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    _logger.ExceptionOccurredWhileExecutingIntegrationHandler(Label, exception);
                }
            }
        }
    }

    private void ThrowIfDisposed()
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(AudioService));
        }
#endif
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _readyTaskCompletionSource.TrySetCanceled();
        _shutdownCancellationTokenSource.Cancel();
        _shutdownCancellationTokenSource.Dispose();

        if (_executeTask is not null)
        {
            try
            {
                await _executeTask.ConfigureAwait(false);
            }
            catch (Exception)
            {
                // ignore
            }
        }
    }
}

file readonly record struct CancellationTokenDisposable(CancellationTokenSource CancellationTokenSource) : IDisposable
{
    public void Dispose() => CancellationTokenSource.Cancel();
}

internal static partial class Logging
{
    [LoggerMessage(1, LogLevel.Debug, "[{Label}] Received an event payload for a non-registered player: {GuildId}.", EventName = nameof(ReceivedEventPayloadForNonRegisteredPlayer))]
    public static partial void ReceivedEventPayloadForNonRegisteredPlayer(this ILogger<LavalinkNode> logger, string label, ulong guildId);

    [LoggerMessage(2, LogLevel.Trace, "[{Label}] Received payload from lavalink node: {Payload}", EventName = nameof(ReceivedPayloadFromLavalinkNode))]
    public static partial void ReceivedPayloadFromLavalinkNode(this ILogger<LavalinkNode> logger, string label, string payload);

    [LoggerMessage(3, LogLevel.Warning, "[{Label}] Multiple ready payloads were received.", EventName = nameof(MultipleReadyPayloadsReceived))]
    public static partial void MultipleReadyPayloadsReceived(this ILogger<LavalinkNode> logger, string label);

    [LoggerMessage(4, LogLevel.Information, "[{Label}] Node is ready (session identifier: {SessionId}).", EventName = nameof(Ready))]
    public static partial void Ready(this ILogger<LavalinkNode> logger, string label, string sessionId);

    [LoggerMessage(5, LogLevel.Warning, "[{Label}] A payload was received before the ready payload was received. The payload will be ignored.", EventName = nameof(PayloadReceivedBeforeReadyPayload))]
    public static partial void PayloadReceivedBeforeReadyPayload(this ILogger<LavalinkNode> logger, string label);

    [LoggerMessage(6, LogLevel.Debug, "[{Label}] Received a player update payload for a non-registered player: {GuildId}.", EventName = nameof(ReceivedPlayerUpdatePayloadForNonRegisteredPlayer))]
    public static partial void ReceivedPlayerUpdatePayloadForNonRegisteredPlayer(this ILogger<LavalinkNode> logger, string label, ulong guildId);

    [LoggerMessage(7, LogLevel.Error, "[{Label}] Exception occurred while executing integration handler.", EventName = nameof(ExceptionOccurredWhileExecutingIntegrationHandler))]
    public static partial void ExceptionOccurredWhileExecutingIntegrationHandler(this ILogger<LavalinkNode> logger, string label, Exception exception);

    [LoggerMessage(8, LogLevel.Error, "[{Label}] Exception occurred during communication.", EventName = nameof(ExceptionOccurredDuringCommunication))]
    public static partial void ExceptionOccurredDuringCommunication(this ILogger<LavalinkNode> logger, string label, Exception exception);
}