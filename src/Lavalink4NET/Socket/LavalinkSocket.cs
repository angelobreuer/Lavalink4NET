namespace Lavalink4NET.Socket;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lavalink4NET.Protocol;
using Lavalink4NET.Protocol.Payloads;
using Lavalink4NET.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class LavalinkSocket : ILavalinkSocket
{
    private readonly Channel<IPayload> _channel;
    private readonly ILogger<LavalinkSocket> _logger;
    private readonly IOptions<LavalinkSocketOptions> _options;
    private bool _disposed;

    public LavalinkSocket(
        ILogger<LavalinkSocket> logger,
        IOptions<LavalinkSocketOptions> options)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(options);

        Label = options.Value.Label ?? $"Lavalink-{CorrelationIdGenerator.GetNextId()}";

        _logger = logger;
        _options = options;
        _channel = Channel.CreateUnbounded<IPayload>();
    }

    public string Label { get; }

    public async ValueTask<IPayload?> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            return await _channel.Reader
                .ReadAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (ChannelClosedException)
        {
            return null;
        }
    }

    public async ValueTask RunAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var buffer = GC.AllocateUninitializedArray<byte>(32 * 1024);

        async ValueTask<WebSocket> ConnectWithRetryAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var attempt = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _logger.LogDebug("[{Label}] Attempting to connect to Lavalink node...", Label);

                var webSocket = new ClientWebSocket();

                var assemblyVersion = typeof(LavalinkSocket).Assembly.GetName().Version;
                webSocket.Options.SetRequestHeader("Authorization", _options.Value.Passphrase);
                webSocket.Options.SetRequestHeader("User-Id", _options.Value.UserId.ToString());
                webSocket.Options.SetRequestHeader("Client-Name", $"Lavalink4NET/{assemblyVersion}");

                try
                {
                    await webSocket
                        .ConnectAsync(_options.Value.Uri, cancellationToken)
                        .ConfigureAwait(false);

                    _logger.LogDebug("[{Label}] Connection to Lavalink node established.", Label);

                    return webSocket;
                }
                catch (Exception exception) when (attempt++ < 10)
                {
                    await Task
                        .Delay(2500, cancellationToken)
                        .ConfigureAwait(false);

                    _logger.LogDebug(exception, "[{Label}] Failed to connect to the Lavalink node.", Label);
                }
            }
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            using var webSocket = await ConnectWithRetryAsync(
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await ReceiveInternalAsync(webSocket, buffer, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task ReceiveInternalAsync(WebSocket webSocket, Memory<byte> receiveBuffer, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(webSocket);
        Debug.Assert(webSocket.State is WebSocketState.Open);

        using var _ = webSocket;

        while (true)
        {
            var receiveResult = await webSocket
                .ReceiveAsync(receiveBuffer, cancellationToken)
                .ConfigureAwait(false);

            if (!receiveResult.EndOfMessage)
            {
                ThrowIfNotEndOfMessage();
            }

            if (receiveResult.MessageType is not WebSocketMessageType.Text)
            {
                if (receiveResult.MessageType is WebSocketMessageType.Close)
                {
                    return;
                }

                ThrowIfInvalidMessageType();
            }

            var buffer = receiveBuffer[..receiveResult.Count];
            await ProcessAsync(buffer, cancellationToken).ConfigureAwait(false);
        }

        [DoesNotReturn]
        static void ThrowIfNotEndOfMessage() => throw new InvalidDataException("Received a partial payload.");

        [DoesNotReturn]
        static void ThrowIfInvalidMessageType() => throw new InvalidDataException("Received bad frame type.");
    }

    private ValueTask ProcessAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var payload = JsonSerializer.Deserialize(
            utf8Json: buffer.Span,
            jsonTypeInfo: ProtocolSerializerContext.Default.IPayload);

        var result = _channel.Writer.TryWrite(payload!);
        Debug.Assert(result);

        return default;
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (disposing)
        {
            _channel.Writer.TryComplete();
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
