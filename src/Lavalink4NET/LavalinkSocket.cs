/*
 *  File:   LavalinkSocket.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2021
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

namespace Lavalink4NET
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Events;
    using Lavalink4NET.Logging;
    using Lavalink4NET.Payloads.Node;
    using Newtonsoft.Json;
    using Payloads;
    using Rest;

    /// <summary>
    ///     The socket for connecting to a lavalink node.
    /// </summary>
    public class LavalinkSocket : LavalinkRestClient, IDisposable
    {
        private readonly IDiscordClientWrapper _client;
        private readonly bool _ioDebug;
        private readonly StringBuilder _overflowBuffer;
        private readonly string _password;
        private readonly Queue<IPayload> _queue;
        private readonly byte[] _receiveBuffer;
        private readonly ReconnectStrategy _reconnectionStrategy;
        private readonly bool _resume;
        private readonly int _sessionTimeout;
        private readonly Uri _webSocketUri;
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _disposed;
        private volatile bool _initialized;
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
            _receiveBuffer = new byte[options.BufferSize];
            _overflowBuffer = new StringBuilder();
            _resume = options.AllowResuming;
            _ioDebug = options.DebugPayloads;
            _sessionTimeout = options.SessionTimeout;
            _queue = new Queue<IPayload>();
            _reconnectionStrategy = options.ReconnectStrategy;
            _cancellationTokenSource = new CancellationTokenSource();
            ResumeKey = Guid.NewGuid().ToString("N");
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
        ///     An asynchronous event which is triggered when a payload was received from the
        ///     lavalink node.
        /// </summary>
        public event AsyncEventHandler<PayloadReceivedEventArgs>? PayloadReceived;

        /// <summary>
        ///     An asynchronous event which is triggered when a new reconnection attempt is made.
        /// </summary>
        public event AsyncEventHandler<ReconnectAttemptEventArgs>? ReconnectAttempt;

        /// <summary>
        ///     Gets a value indicating whether the client is connected to the lavalink node.
        /// </summary>
        public bool IsConnected => _webSocket != null
            && _webSocket.State == WebSocketState.Open;

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
            _webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(5);

            // add resume header
            if (_resume && _initialized)
            {
                Logger?.Log(this, string.Format("Trying to resume Lavalink Session ... Key: {0}.", ResumeKey), LogLevel.Debug);
                _webSocket.Options.SetRequestHeader("Resume-Key", ResumeKey);
            }

            try
            {
                // connect to the lavalink node
                await _webSocket.ConnectAsync(_webSocketUri, cancellationToken);
            }
            catch (Exception ex)
            {
                if (Logger is null)
                {
                    throw;
                }

                Logger.Log(this, string.Format("Connection to Lavalink Node `{0}` failed!", _webSocketUri), LogLevel.Error, ex);
                return;
            }

            // replay payloads
            if (_queue.Count > 0)
            {
                Logger?.Log(this, string.Format("Replaying {0} payload(s)...", _queue.Count), LogLevel.Debug);

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

                if (_resume && _initialized)
                {
                    Logger?.Log(this, string.Format("{0} to Lavalink Node, Resume Key: {1}!", type, ResumeKey));
                }
                else
                {
                    Logger?.Log(this, string.Format("{0} to Lavalink Node!", type));
                }
            }

            // trigger (re)connected event
            await OnConnectedAsync(new ConnectedEventArgs(_webSocketUri, _initialized));

            // resume next session
            _initialized = true;

            // send configure resuming payload
            if (_resume)
            {
                await SendPayloadAsync(new ConfigureResumingPayload(ResumeKey, _sessionTimeout));
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
            await _client.InitializeAsync();

            // connect to the node
            await ConnectAsync();

            // start life-cycle
            _ = RunLifeCycleAsync();

            _initialized = true;
        }

        /// <summary>
        ///     Sends a payload to the lavalink node asynchronously.
        /// </summary>
        /// <param name="payload">the payload to sent</param>
        /// <param name="forceSend">
        ///     a value indicating whether an exception should be thrown if the connection is
        ///     closed. If <see langword="true"/>, an exception is thrown; Otherwise payloads will
        ///     be stored into a send queue and will be replayed (FIFO) after successful reconnection.
        /// </param>
        /// <returns>a task that represents the asynchronous operation</returns>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the connection to the node is closed (see: <see cref="IsConnected"/>) and
        ///     <paramref name="forceSend"/> is <see langword="true"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the node socket has not been initialized. (Call <see
        ///     cref="InitializeAsync"/> before sending payloads)
        /// </exception>
        /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
        public async Task SendPayloadAsync(IPayload payload, bool forceSend = false)
        {
            EnsureNotDisposed();
            EnsureInitialized();

            if (!IsConnected)
            {
                if (forceSend)
                {
                    throw new InvalidOperationException("The connection is closed.");
                }

                // store payload into send queue, so the events will be replayed when reconnected
                _queue.Enqueue(payload);
                return;
            }

            // serialize the payload to json
            var content = JsonConvert.SerializeObject(payload);

            // rent a buffer from the shared array pool
            var pooledBuffer = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(content.Length));

            try
            {
                // encode the payload into the buffer
                var length = Encoding.UTF8.GetBytes(content, 0, content.Length, pooledBuffer, 0);

                // send the payload
                await _webSocket!.SendAsync(new ArraySegment<byte>(pooledBuffer, 0, length), WebSocketMessageType.Text, true, _cancellationTokenSource!.Token);
            }
            catch (Exception ex)
            {
                Logger?.Log(ex, "Failed to send payload. Trying to reconnect to node...");

                // enqueue packet that failed to sent
                _queue.Enqueue(payload);
            }
            finally
            {
                // return the buffer to its pool for later reuse
                ArrayPool<byte>.Shared.Return(pooledBuffer);
            }

            if (_ioDebug)
            {
                Logger?.Log(this, string.Format("Sent payload `{0}` to {1}.", content, _webSocketUri), LogLevel.Trace);
            }
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
        ///     thrown if the node socket has not been initialized. (Call <see
        ///     cref="InitializeAsync"/> before sending payloads)
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
        private async Task ProcessNextPayload()
        {
            EnsureNotDisposed();

            WebSocketReceiveResult result;

            try
            {
                result = await _webSocket!.ReceiveAsync(new ArraySegment<byte>(_receiveBuffer, 0, _receiveBuffer.Length), _cancellationTokenSource!.Token);
            }
            catch (WebSocketException ex)
            {
                if (_cancellationTokenSource!.IsCancellationRequested)
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
            if (result.MessageType == WebSocketMessageType.Close)
            {
                Logger?.Log(this, string.Format("Lavalink Node disconnected: {0}, {1}.", result.CloseStatus.GetValueOrDefault(), result.CloseStatusDescription), LogLevel.Warning);
                await OnDisconnectedAsync(new DisconnectedEventArgs(_webSocketUri, result.CloseStatus.GetValueOrDefault(), result.CloseStatusDescription, true));

                _webSocket.Dispose();
                _webSocket = null;
                return;
            }

            var content = Encoding.UTF8.GetString(_receiveBuffer, 0, result.Count);

            if (!result.EndOfMessage)
            {
                // the server sent a message frame that is incomplete
                _overflowBuffer.Append(content);
                return;
            }

            // check if old data exists
            if (result.EndOfMessage && _overflowBuffer.Length > 0)
            {
                _overflowBuffer.Append(content);
                content = _overflowBuffer.ToString();
                _overflowBuffer.Clear();
            }

            if (_ioDebug)
            {
                Logger?.Log(this, string.Format("Received payload: `{0}` from: {1}.", content, _webSocketUri), LogLevel.Trace);
            }

            // process data
            try
            {
                var payload = PayloadConverter.ReadPayload(content);
                var eventArgs = new PayloadReceivedEventArgs(payload, content);
                await OnPayloadReceived(eventArgs);
            }
            catch (Exception ex)
            {
                Logger?.Log(this, string.Format("Received bad payload: {0}.", content), LogLevel.Warning, ex);
                await CloseAsync(WebSocketCloseStatus.InvalidPayloadData);
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

            while (!_cancellationTokenSource!.IsCancellationRequested)
            {
                // receive new payload until the web-socket is connected.
                while (IsConnected && !_cancellationTokenSource.IsCancellationRequested)
                {
                    await ProcessNextPayload();
                }

                var lostConnectionAt = DateTimeOffset.UtcNow;

                // try reconnect
                for (var attempt = 1; !_cancellationTokenSource.IsCancellationRequested; attempt++)
                {
                    // reconnect
                    await ConnectAsync();

                    if (IsConnected)
                    {
                        // reconnection successful
                        break;
                    }

                    var eventArgs = new ReconnectAttemptEventArgs(_webSocketUri, attempt, _reconnectionStrategy);
                    await OnReconnectAttemptAsync(eventArgs);

                    // give up
                    if (eventArgs.CancelReconnect)
                    {
                        Logger?.Log(this, "Reconnection aborted due event.");
                        return;
                    }

                    // add delay between reconnects
                    var delay = _reconnectionStrategy(lostConnectionAt, attempt);

                    // reconnection give up
                    if (delay is null)
                    {
                        Logger?.Log(this, "Reconnection failed! .. giving up.", LogLevel.Warning);
                        return;
                    }

                    Logger?.Log(this, string.Format("Waiting {0} before next reconnect attempt...", delay.Value), LogLevel.Debug);
                    await Task.Delay(delay.Value);
                }
            }
        }
    }
}
