/*
 *  File:   LavalinkSocket.cs
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

namespace Lavalink4NET;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Events;
using Lavalink4NET.Integrations;
using Lavalink4NET.Logging;
using Lavalink4NET.Payloads.Node;
using Payloads;
using Rest;

/// <summary>
///     The socket for connecting to a lavalink node.
/// </summary>
public class LavalinkSocket : LavalinkRestClient, IDisposable
{
    private readonly IDiscordClientWrapper _client;
    private readonly bool _ioDebug;
    private readonly string _password;
    private readonly Queue<JsonNode> _queue;
    private readonly byte[] _receiveBuffer;
    private readonly ReconnectStrategy _reconnectionStrategy;
    private readonly bool _resume;
    private readonly int _sessionTimeout;
    private readonly Uri _webSocketUri;
    private readonly bool _suppressReconnectionEntries;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _disposed;
    private volatile bool _initialized;
    private IIntegrationCollection? _integrations;
    private bool _mayResume;
    private bool _previousConnectionAttemptFailed;
    private ClientWebSocket? _webSocket;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LavalinkSocket"/> class.
    /// </summary>
    /// <param name="options">the node options</param>
    /// <param name="client">the discord client</param>
    /// <param name="logger">the logger</param>
    /// <param name="cache">an optional cache that caches track requests</param>
    public LavalinkSocket(LavalinkNodeOptions options, IDiscordClientWrapper client, ILogger? logger = null, ILavalinkCache? cache = null)
        : base(options, logger, cache)
    {
        Logger = logger;

        if (options.BufferSize <= 0)
        {
            if (Logger is null)
            {
                throw new InvalidOperationException("The specified buffer size is zero or negative.");
            }

            Logger.Log(this, "The specified buffer size is zero or negative .. using 1048576 (1MiB).", LogLevel.Warning);
            options.BufferSize = 1024 * 1024; // 1 MiB buffer size
        }

        if (options.ReconnectStrategy is null)
        {
            throw new InvalidOperationException("No reconnection strategy specified in options.");
        }

        if (options.AllowResuming && options.SessionTimeout < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options.SessionTimeout), options.SessionTimeout,
                "The specified session timeout can not be negative or zero.");
        }

        _client = client;
        _password = options.Password;
        _webSocketUri = new Uri(options.WebSocketUri);
        _resume = options.AllowResuming;
        _ioDebug = options.DebugPayloads;
        _sessionTimeout = options.SessionTimeout;
        _queue = new Queue<JsonNode>();
        _reconnectionStrategy = options.ReconnectStrategy;
        _cancellationTokenSource = new CancellationTokenSource();
        _mayResume = options.ResumeKey is not null;
        _suppressReconnectionEntries = options.SuppressReconnectionEntries;

        ResumeKey = options.ResumeKey ?? Guid.NewGuid().ToString("N");

#if NET6_0_OR_GREATER
        _receiveBuffer = GC.AllocateUninitializedArray<byte>(options.BufferSize);
#else
        _receiveBuffer = new byte[options.BufferSize];
#endif
    }

    /// <summary>
    ///     Asynchronously triggered when the socket connected to a remote endpoint.
    /// </summary>
    public event AsyncEventHandler<ConnectedEventArgs>? Connected;

    /// <summary>
    ///     Asynchronously triggered when the socket disconnected from the remote endpoint.
    /// </summary>
    public event AsyncEventHandler<DisconnectedEventArgs>? Disconnected;

    /// <summary>
    ///     An asynchronous event which is triggered when a payload was received from the lavalink node.
    /// </summary>
    public event AsyncEventHandler<PayloadReceivedEventArgs>? PayloadReceived;

    /// <summary>
    ///     An asynchronous event which is triggered when a new reconnection attempt is made.
    /// </summary>
    public event AsyncEventHandler<ReconnectAttemptEventArgs>? ReconnectAttempt;

    public IIntegrationCollection Integrations
    {
        get => _integrations ??= new IntegrationCollection();
        protected set => _integrations = value;
    }

    /// <summary>
    ///     Gets a value indicating whether the client is connected to the lavalink node.
    /// </summary>
    public bool IsConnected => _webSocket is not null && _webSocket.State is WebSocketState.Open;

    /// <summary>
    ///     Gets the logger.
    /// </summary>
    public ILogger? Logger { get; }

    /// <summary>
    ///     Gets the resume key.
    /// </summary>
    public string ResumeKey { get; }

    /// <summary>
    ///     Closes the connection to the remote endpoint asynchronously.
    /// </summary>
    /// <param name="closeStatus">the close status</param>
    /// <param name="reason">the close reason</param>
    /// <param name="cancellationToken">
    ///     a cancellation token used to propagate notification that this operation should be canceled.
    /// </param>
    /// <returns>a task that represents the asynchronously operation.</returns>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public async Task CloseAsync(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.Empty, string reason = "", CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();

        cancellationToken.ThrowIfCancellationRequested();

        if (_webSocket is null)
        {
            throw new InvalidOperationException("Connection not open.");
        }

        // close connection
        await _webSocket.CloseAsync(closeStatus, reason, cancellationToken);

        // dispose web socket
        _webSocket.Dispose();
        _webSocket = null;

        // trigger event
        await OnDisconnectedAsync(new DisconnectedEventArgs(_webSocketUri, closeStatus, reason, false));
    }
    /// <summary>
    ///     Connects to the lavalink node asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    ///     a cancellation token used to propagate notification that the operation should be canceled.
    /// </param>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the connection is already open
    /// </exception>
    /// <exception cref="OperationCanceledException">thrown if the operation was canceled</exception>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();

        cancellationToken.ThrowIfCancellationRequested();

        if (IsConnected)
        {
            throw new InvalidOperationException("Connection is already open.");
        }

        // initialize web-socket and set headers required for the node
        _webSocket = new ClientWebSocket();
        _webSocket.Options.SetRequestHeader("Authorization", _password);
        _webSocket.Options.SetRequestHeader("Num-Shards", _client.ShardCount.ToString());
        _webSocket.Options.SetRequestHeader("User-Id", _client.CurrentUserId.ToString());
        _webSocket.Options.SetRequestHeader("Client-Name", "Lavalink4NET");
        _webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(5);

        // add resume header
        if (_mayResume)
        {
            if (!_suppressReconnectionEntries || !_previousConnectionAttemptFailed)
            {
                Logger?.Log(this, string.Format("Trying to resume Lavalink Session ... Key: {0}.", ResumeKey), LogLevel.Debug);
            }
            _webSocket.Options.SetRequestHeader("Resume-Key", ResumeKey);
        }

        try
        {
            // connect to the lavalink node
            await _webSocket.ConnectAsync(_webSocketUri, cancellationToken);
            _previousConnectionAttemptFailed = false;
        }
        catch (Exception ex)
        {
            if (Logger is null)
            {
                throw;
            }

            if (!_suppressReconnectionEntries || !_previousConnectionAttemptFailed)
            {
                Logger.Log(this, string.Format("Connection to Lavalink Node `{0}` failed!", _webSocketUri), LogLevel.Error, ex);
            }

            _previousConnectionAttemptFailed = true;
            return;
        }

        // replay payloads
        if (_queue.Count > 0)
        {
            Logger?.Log(this, string.Format("Replaying {0} payload(s) to `{1}`...", _queue.Count, _webSocketUri), LogLevel.Debug);

            // replay (FIFO)
            while (_queue.Count > 0)
            {
                await SendPayloadAsync(_queue.Dequeue());
            }
        }

        // log "Connected"-message
        if (Logger != null)
        {
            var type = _initialized ? "Reconnected" : "Connected";

            if (_mayResume)
            {
                Logger?.Log(this, string.Format("{0} to Lavalink Node `{1}`, Resume Key: {2}!", type, _webSocketUri, ResumeKey));
            }
            else
            {
                Logger?.Log(this, string.Format("{0} to Lavalink Node `{1}`!", type, _webSocketUri));
            }
        }

        // trigger (re)connected event
        await OnConnectedAsync(new ConnectedEventArgs(_webSocketUri, _initialized));

        // resume next session
        _initialized = true;
        _mayResume = true;

        // send configure resuming payload
        if (_resume)
        {
            var configureResumingPayload = new ConfigureResumingPayload
            {
                Key = ResumeKey,
                Timeout = _sessionTimeout,
            };

            await SendPayloadAsync(OpCode.ConfigureResuming, configureResumingPayload).ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Disposes the inner web socket.
    /// </summary>
    public override void Dispose()
    {
        if (!_initialized || _disposed)
        {
            return;
        }

        _disposed = true;

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _webSocket?.Dispose();

        _cancellationTokenSource = null;
        _webSocket = null;

        base.Dispose();
    }

    /// <summary>
    ///     Initializes the node asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    public async Task InitializeAsync()
    {
        EnsureNotDisposed();

        if (_initialized)
        {
            return;
        }

        // await discord client initialization
        await _client.InitializeAsync().ConfigureAwait(false);

        // connect to the node
        await ConnectAsync().ConfigureAwait(false);

        // start life-cycle
        _ = RunLifeCycleAsync();

        _initialized = true;
    }

    public async ValueTask SendPayloadAsync<T>(OpCode opCode, T payload, bool forceSend = false, CancellationToken cancellationToken = default)
        where T : notnull
    {
        var node = JsonSerializer.SerializeToNode(payload)!;
        node["op"] = opCode.Value;

        foreach (var pair in Integrations)
        {
            await pair.Value
                .InterceptPayloadAsync(node, cancellationToken)
                .ConfigureAwait(false);
        }

        await SendPayloadAsync(node, forceSend, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Notifies a player disconnect asynchronously.
    /// </summary>
    /// <param name="eventArgs">the event arguments passed with the event</param>
    /// <returns>a task that represents the asynchronously operation.</returns>
    protected internal virtual Task NotifyDisconnectAsync(PlayerDisconnectedEventArgs eventArgs)
        => Task.CompletedTask;

    /// <summary>
    ///     Ensures that the socket is initialized.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the node socket has not been initialized. (Call <see cref="InitializeAsync"/>
    ///     before sending payloads)
    /// </exception>
    protected void EnsureInitialized()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("The node socket has not been initialized. Call 'InitializeAsync()' before sending payloads.");
        }
    }

    /// <summary>
    ///     Triggers the <see cref="Connected"/> event asynchronously.
    /// </summary>
    /// <param name="eventArgs">the event arguments</param>
    /// <returns>a task that represents the asynchronously operation.</returns>
    protected virtual Task OnConnectedAsync(ConnectedEventArgs eventArgs)
        => Connected.InvokeAsync(this, eventArgs);

    /// <summary>
    ///     Triggers the <see cref="Disconnected"/> event asynchronously.
    /// </summary>
    /// <param name="eventArgs">the event arguments</param>
    /// <returns>a task that represents the asynchronously operation.</returns>
    protected virtual Task OnDisconnectedAsync(DisconnectedEventArgs eventArgs)
        => Disconnected.InvokeAsync(this, eventArgs);

    /// <summary>
    ///     Invokes the <see cref="PayloadReceived"/> event asynchronously. (Can be override for
    ///     event catching)
    /// </summary>
    /// <param name="eventArgs">the event arguments for the event</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    protected virtual Task OnPayloadReceived(PayloadReceivedEventArgs eventArgs)
        => PayloadReceived.InvokeAsync(this, eventArgs);

    /// <summary>
    ///     Triggers the <see cref="ReconnectAttempt"/> event asynchronously.
    /// </summary>
    /// <param name="eventArgs">the event arguments</param>
    /// <returns>a task that represents the asynchronously operation.</returns>
    protected virtual Task OnReconnectAttemptAsync(ReconnectAttemptEventArgs eventArgs)
        => ReconnectAttempt.InvokeAsync(this, eventArgs);

    protected virtual ValueTask ProcessPayloadAsync(PayloadContext payloadContext, CancellationToken cancellationToken = default) => default;

    /// <summary>
    ///     Throws an exception if the <see cref="LavalinkSocket"/> instance is disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    private void EnsureNotDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(LavalinkSocket));
        }
    }

    /// <summary>
    ///     Processes an incoming payload asynchronously.
    /// </summary>
    /// <remarks>
    ///     This method should not be called manually. It is called in the connection life cycle,
    ///     see: <see cref="RunLifeCycleAsync"/>.
    /// </remarks>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    private async Task ProcessNextPayload(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();

        WebSocketReceiveResult result;

        try
        {
            result = await _webSocket!
                .ReceiveAsync(new ArraySegment<byte>(_receiveBuffer, 0, _receiveBuffer.Length), cancellationToken)
                .ConfigureAwait(false);
        }
        catch (WebSocketException ex)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            Logger?.Log(ex, "Lavalink Node disconnected (without handshake, maybe connection loss or server crash)");
            await OnDisconnectedAsync(new DisconnectedEventArgs(_webSocketUri, WebSocketCloseStatus.Empty, string.Empty, true));

            _webSocket?.Dispose();
            _webSocket = null;
            return;
        }

        // check if the web socket received a close frame
        if (result.MessageType is WebSocketMessageType.Close)
        {
            Logger?.Log(this, string.Format("Lavalink Node `{0}` disconnected: {1}, {2}.", _webSocketUri, result.CloseStatus.GetValueOrDefault(), result.CloseStatusDescription), LogLevel.Warning);
            await OnDisconnectedAsync(new DisconnectedEventArgs(_webSocketUri, result.CloseStatus.GetValueOrDefault(), result.CloseStatusDescription, true));

            _webSocket.Dispose();
            _webSocket = null;
            return;
        }

        if (!result.EndOfMessage)
        {
            // the server sent a message frame that is incomplete
            await CloseAsync(WebSocketCloseStatus.PolicyViolation, "An incomplete frame was received.", CancellationToken.None).ConfigureAwait(false);
            return;
        }

        var payload = _receiveBuffer.AsMemory(0, result.Count);

        if (_ioDebug && Logger is not null)
        {
            var content = Encoding.UTF8.GetString(_receiveBuffer, 0, result.Count);
            Logger.Log(this, string.Format("Received payload: `{0}` from: {1}.", content, _webSocketUri), LogLevel.Trace);
        }

        var jsonDocument = JsonDocument.Parse(payload);

        if (!jsonDocument.RootElement.TryGetProperty("op", out var opCodeToken))
        {
            throw new JsonException("Invalid JSON: Expected 'op' in json object.");
        }

        var opCode = opCodeToken.Deserialize<OpCode>();
        var eventType = default(EventType?);

        if (opCode == OpCode.Event)
        {
            if (!jsonDocument.RootElement.TryGetProperty("type", out var eventTypeToken))
            {
                throw new JsonException("Invalid JSON: Expected 'type' in json object.");
            }

            eventType = eventTypeToken.Deserialize<EventType>();
        }

        var guildId = jsonDocument.RootElement.TryGetProperty("guildId", out var guildIdProperty)
            ? ulong.Parse(guildIdProperty.GetString()!, provider: CultureInfo.InvariantCulture)
            : default(ulong?);

        var payloadContext = new PayloadContext(this, opCode, eventType, jsonDocument.RootElement, guildId);

        // process data
        try
        {
            var payloadHandled = false;
            foreach (var pair in Integrations)
            {
                payloadHandled = await pair.Value
                    .ProcessPayloadAsync(payloadContext, cancellationToken)
                    .ConfigureAwait(false);

                if (payloadHandled)
                {
                    break;
                }
            }

            if (!payloadHandled)
            {
                await ProcessPayloadAsync(payloadContext, cancellationToken).ConfigureAwait(false);
            }

            var eventArgs = new PayloadReceivedEventArgs(payloadContext);
            await OnPayloadReceived(eventArgs).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (Logger is not null)
            {
                var content = Encoding.UTF8.GetString(_receiveBuffer, 0, result.Count);
                Logger.Log(this, string.Format("Received bad payload from `{0}`: {1}.", _webSocketUri, content), LogLevel.Warning, ex);
            }

            await CloseAsync(WebSocketCloseStatus.InvalidPayloadData).ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Runs the receive / reconnect life cycle asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    private async Task RunLifeCycleAsync()
    {
        EnsureNotDisposed();

        var cancellationToken = _cancellationTokenSource!.Token;
        while (!cancellationToken.IsCancellationRequested)
        {
            // receive new payload until the web-socket is connected.
            while (IsConnected && !cancellationToken.IsCancellationRequested)
            {
                await ProcessNextPayload(cancellationToken).ConfigureAwait(false);
            }

            var lostConnectionAt = DateTimeOffset.UtcNow;

            // try reconnect
            for (var attempt = 1; !cancellationToken.IsCancellationRequested; attempt++)
            {
                // reconnect
                await ConnectAsync(cancellationToken).ConfigureAwait(false);

                if (IsConnected)
                {
                    // reconnection successful
                    break;
                }

                var eventArgs = new ReconnectAttemptEventArgs(_webSocketUri, attempt, _reconnectionStrategy);
                await OnReconnectAttemptAsync(eventArgs).ConfigureAwait(false);

                // give up
                if (eventArgs.CancelReconnect)
                {
                    Logger?.Log(this, string.Format("Reconnection to {0} aborted due event.", _webSocketUri));
                    return;
                }

                // add delay between reconnects
                var delay = _reconnectionStrategy(lostConnectionAt, attempt);

                // reconnection give up
                if (delay is null)
                {
                    Logger?.Log(this, string.Format("Reconnection to `{0}` failed! .. giving up.", _webSocketUri), LogLevel.Warning);
                    return;
                }

                if (!_suppressReconnectionEntries)
                {
                    Logger?.Log(this, string.Format("Waiting {0} before next reconnect attempt to `{1}`...", delay.Value, _webSocketUri), LogLevel.Debug);
                }

                await Task
                    .Delay(delay.Value, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    ///     Sends a payload to the lavalink node asynchronously.
    /// </summary>
    /// <param name="payload">the payload to sent</param>
    /// <param name="forceSend">
    ///     a value indicating whether an exception should be thrown if the connection is closed. If
    ///     <see langword="true"/>, an exception is thrown; Otherwise payloads will be stored into a
    ///     send queue and will be replayed (FIFO) after successful reconnection.
    /// </param>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the connection to the node is closed (see: <see cref="IsConnected"/>) and
    ///     <paramref name="forceSend"/> is <see langword="true"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     thrown if the node socket has not been initialized. (Call <see cref="InitializeAsync"/>
    ///     before sending payloads)
    /// </exception>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    private async ValueTask SendPayloadAsync(JsonNode node, bool forceSend = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();
        EnsureInitialized();

        using var cancellationTokenSource = cancellationToken.CanBeCanceled
            ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource!.Token)
            : null;

        if (cancellationTokenSource is not null)
        {
            cancellationToken = cancellationTokenSource.Token;
        }

        if (!IsConnected)
        {
            if (forceSend)
            {
                throw new InvalidOperationException("The connection is closed.");
            }

            // store payload into send queue, so the events will be replayed when reconnected
            _queue.Enqueue(node);
            return;
        }

        using var pooledBufferWriter = new PooledBufferWriter();
        using (var utf8JsonWriter = new Utf8JsonWriter(pooledBufferWriter))
        {
            JsonSerializer.Serialize(utf8JsonWriter, node);
        }

        try
        {
            // send the payload
            await _webSocket!.SendAsync(
                buffer: pooledBufferWriter.WrittenSegment,
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            Logger?.Log(exception, string.Format("Failed to send payload. Trying to reconnect to node `{0}`...", _webSocketUri));

            // enqueue packet that failed to sent
            _queue.Enqueue(node);
        }

        if (_ioDebug)
        {
            var message = string.Format(
                "Sent payload `{0}` to {1}.",
                Encoding.UTF8.GetString(pooledBufferWriter.WrittenSpan.ToArray()),
                _webSocketUri);

            Logger?.Log(this, message, LogLevel.Trace);
        }
    }
}
