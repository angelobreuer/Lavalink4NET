namespace Lavalink4NET.Socket;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
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
	private readonly IHttpMessageHandlerFactory _httpMessageHandlerFactory;
	private readonly ILogger<LavalinkSocket> _logger;
	private readonly IOptions<LavalinkSocketOptions> _options;
	private bool _disposed;

	public LavalinkSocket(
		IHttpMessageHandlerFactory httpMessageHandlerFactory,
		ILogger<LavalinkSocket> logger,
		IOptions<LavalinkSocketOptions> options)
	{
		ArgumentNullException.ThrowIfNull(httpMessageHandlerFactory);
		ArgumentNullException.ThrowIfNull(logger);
		ArgumentNullException.ThrowIfNull(options);

		Label = options.Value.Label ?? $"Lavalink-{CorrelationIdGenerator.GetNextId()}";
		_httpMessageHandlerFactory = httpMessageHandlerFactory;
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
		catch (ChannelClosedException exception) when (exception.InnerException is not null)
		{
			throw exception.InnerException;
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

				_logger.AttemptingToConnect(Label);

				var webSocket = new ClientWebSocket();

				var assemblyVersion = typeof(LavalinkSocket).Assembly.GetName().Version;
				webSocket.Options.SetRequestHeader("Authorization", _options.Value.Passphrase);
				webSocket.Options.SetRequestHeader("User-Id", _options.Value.UserId.ToString());
				webSocket.Options.SetRequestHeader("Client-Name", $"Lavalink4NET/{assemblyVersion}");

				if (_options.Value.SessionId is not null)
				{
					webSocket.Options.SetRequestHeader("Session-Id", _options.Value.SessionId);
				}

				try
				{
#if NET7_0_OR_GREATER
					var httpMessageHandler = _httpMessageHandlerFactory.CreateHandler(_options.Value.HttpClientName);
					var httpMessageInvoker = new HttpMessageInvoker(httpMessageHandler, disposeHandler: true);

					await webSocket
						.ConnectAsync(_options.Value.Uri, httpMessageInvoker, cancellationToken)
						.ConfigureAwait(false);
#else
                    await webSocket
                        .ConnectAsync(_options.Value.Uri, cancellationToken)
                        .ConfigureAwait(false);
#endif

					_logger.ConnectionEstablished(Label);

					return webSocket;
				}
				catch (Exception exception) when (attempt++ < 10)
				{
					await Task
						.Delay(2500, cancellationToken)
						.ConfigureAwait(false);

					_logger.FailedToConnect(Label, exception);
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

		try
		{
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
		}
		catch (Exception exception)
		{
			_channel.Writer.TryComplete(exception);
		}
		finally
		{
			_channel.Writer.TryComplete();
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

internal static partial class Logging
{
	[LoggerMessage(1, LogLevel.Debug, "[{Label}] Attempting to connect to Lavalink node...", EventName = nameof(AttemptingToConnect))]
	public static partial void AttemptingToConnect(this ILogger<LavalinkSocket> logger, string label);

	[LoggerMessage(2, LogLevel.Debug, "[{Label}] Connection to Lavalink node established.", EventName = nameof(ConnectionEstablished))]
	public static partial void ConnectionEstablished(this ILogger<LavalinkSocket> logger, string label);

	[LoggerMessage(3, LogLevel.Debug, "[{Label}] Failed to connect to the Lavalink node.", EventName = nameof(FailedToConnect))]
	public static partial void FailedToConnect(this ILogger<LavalinkSocket> logger, string label, Exception exception);
}