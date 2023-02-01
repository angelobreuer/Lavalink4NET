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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class LavalinkSocket : IDisposable
{
    private readonly Channel<IPayload> _channel;
    private readonly ILogger<LavalinkSocket> _logger;
    private readonly IOptions<LavalinkSocketOptions> _options;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private bool _disposed;

    public LavalinkSocket(
        ILogger<LavalinkSocket> logger,
        IOptions<LavalinkSocketOptions> options)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(options);

        _logger = logger;
        _options = options;

        _channel = Channel.CreateUnbounded<IPayload>();
        _cancellationTokenSource = new CancellationTokenSource();

        _ = RunInternalAsync(_cancellationTokenSource.Token);
    }

    public ValueTask<IPayload> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _channel.Reader.ReadAsync(cancellationToken);
    }

    private async Task RunInternalAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var buffer = GC.AllocateUninitializedArray<byte>(32 * 1024);

        static async ValueTask<WebSocket> ConnectWithRetryAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(uri);

            var attempt = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var webSocket = new ClientWebSocket();

                try
                {
                    await webSocket
                        .ConnectAsync(uri, cancellationToken)
                        .ConfigureAwait(false);

                    return webSocket;
                }
                catch (Exception) when (attempt++ < 10)
                {
                    continue;
                }
            }
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            using var webSocket = await ConnectWithRetryAsync(
                uri: _options.Value.Uri,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await ReceiveInternalAsync(webSocket, buffer, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task ReceiveInternalAsync(WebSocket webSocket, Memory<byte> receiveBuffer, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(webSocket);

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
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
