/*
 *  File:   LavalinkSocket.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2019
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
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Payloads;
    using Rest;

    /// <summary>
    ///     The socket for connecting to a lavalink node.
    /// </summary>
    public class LavalinkSocket : LavalinkRestClient, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IDiscordClientWrapper _client;
        private readonly bool _ioDebug;
        private readonly StringBuilder _overflowBuffer;
        private readonly string _password;
        private readonly Queue<IPayload> _queue;
        private readonly byte[] _receiveBuffer;
        private readonly bool _resume;
        private readonly Guid _resumeKey;
        private readonly Uri _webSocketUri;
        private volatile bool _initialized;
        private ClientWebSocket _webSocket;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LavalinkSocket"/> class.
        /// </summary>
        /// <param name="options">the node options</param>
        /// <param name="client">the discord client</param>
        /// <param name="logger">the logger</param>
        public LavalinkSocket(LavalinkNodeOptions options, IDiscordClientWrapper client, ILogger<Lavalink> logger)
            : base(options, logger)
        {
            Logger = logger;

            if (options.BufferSize <= 0)
            {
                if (Logger == null)
                {
                    throw new InvalidOperationException("The specified buffer size is zero or negative.");
                }

                Logger.LogWarning("The specified buffer size is zero or negative .. using 1048576 (1MiB).");
                options.BufferSize = 1024 * 1024; // 1 MiB buffer size
            }

            _client = client;
            _password = options.Password;
            _webSocketUri = new Uri(options.WebSocketUri);
            _receiveBuffer = new byte[options.BufferSize];
            _overflowBuffer = new StringBuilder();
            _resume = options.AllowResuming;
            _ioDebug = options.DebugPayloads;
            _resumeKey = Guid.NewGuid();
            _queue = new Queue<IPayload>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        ///     An asynchronous event which is triggered when a payload was received from the
        ///     lavalink node.
        /// </summary>
        public event AsyncEventHandler<PayloadReceivedEventArgs> PayloadReceived;

        /// <summary>
        ///     Gets a value indicating whether the client is connected to the lavalink node.
        /// </summary>
        public bool IsConnected => _webSocket != null
            && _webSocket.State == WebSocketState.Open;

        /// <summary>
        ///     Gets the logger.
        /// </summary>
        public ILogger<Lavalink> Logger { get; }

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
        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
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
                Logger?.LogInformation("Trying to resume Lavalink Session ... Key: {SessionKey}.", _resumeKey);
                _webSocket.Options.SetRequestHeader("Resume-Key", _resumeKey.ToString());
            }

            try
            {
                // connect to the lavalink node
                await _webSocket.ConnectAsync(_webSocketUri, cancellationToken);
            }
            catch (Exception ex)
            {
                if (Logger == null)
                {
                    throw;
                }

                Logger.LogError(ex, "Connection to Lavalink Node `{Uri}` failed!", _webSocketUri);
                return;
            }

            // replay payloads
            if (_queue.Count > 0)
            {
                Logger?.LogInformation("Replaying {Count} payload(s)...", _queue.Count);

                // replay (FIFO)
                while (_queue.TryDequeue(out var payload))
                {
                    await SendPayloadAsync(payload);
                }
            }

            // log "Connected"-message
            if (Logger != null)
            {
                var type = _initialized ? "Reconnected" : "Connected";

                if (_resume && _initialized)
                {
                    Logger?.LogInformation("{Type} to Lavalink Node, Resume Key: {Key}!", type, _resumeKey);
                }
                else
                {
                    Logger?.LogInformation("{Type} to Lavalink Node!", type);
                }
            }

            // resume next session
            _initialized = true;

            // send configure resuming payload
            if (_resume)
            {
                await SendPayloadAsync(new ConfigureResumingPayload(_resumeKey.ToString()));
            }
        }

        /// <summary>
        ///     Disposes the inner web socket.
        /// </summary>
        public override void Dispose()
        {
            if (!_initialized)
            {
                return;
            }

            _cancellationTokenSource.Cancel();
            _webSocket?.Dispose();

            base.Dispose();
        }

        /// <summary>
        ///     Initializes the node asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        public async Task InitializeAsync()
        {
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
        ///     a value indicating whether an exception should be thrown if the connection is closed.
        ///     If <see langword="true"/>, an exception is thrown; Otherwise payloads will be stored
        ///     into a send queue and will be replayed (FIFO) after successful reconnection.
        /// </param>
        /// <returns>a task that represents the asynchronous operation</returns>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the connection to the node is closed (see: <see cref="IsConnected"/>) and
        ///     <paramref name="forceSend"/> is <see langword="true"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the node socket has not been initialized. (Call
        ///     <see cref="InitializeAsync"/> before sending payloads)
        /// </exception>
        public async Task SendPayloadAsync(IPayload payload, bool forceSend = false)
        {
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
                await _webSocket.SendAsync(new ArraySegment<byte>(pooledBuffer, 0, length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            finally
            {
                // return the buffer to its pool for later reuse
                ArrayPool<byte>.Shared.Return(pooledBuffer);
            }

            if (_ioDebug)
            {
                Logger?.LogTrace("Sent payload `{Payload}` to {Uri}.", content, _webSocketUri);
            }
        }

        /// <summary>
        ///     Ensures that the socket is initialized.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the node socket has not been initialized. (Call
        ///     <see cref="InitializeAsync"/> before sending payloads)
        /// </exception>
        protected void EnsureInitialized()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("The node socket has not been initialized. Call 'InitializeAsync()' before sending payloads.");
            }
        }

        /// <summary>
        ///     Invokes the <see cref="PayloadReceived"/> event asynchronously. (Can be override for
        ///     event catching)
        /// </summary>
        /// <param name="payload">the received payload</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        protected virtual Task OnPayloadReceived(IPayload payload)
            => PayloadReceived.InvokeAsync(this, new PayloadReceivedEventArgs(payload));

        /// <summary>
        ///     Processes an incoming payload asynchronously.
        /// </summary>
        /// <remarks>
        ///     This method should not be called manually. It is called in the connection life cycle,
        ///     see: <see cref="RunLifeCycleAsync"/>.
        /// </remarks>
        /// <returns>a task that represents the asynchronous operation</returns>
        private async Task ProcessNextPayload()
        {
            WebSocketReceiveResult result;

            try
            {
                result = await _webSocket.ReceiveAsync(_receiveBuffer, CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                Logger?.LogWarning(ex, "Lavalink Node disconnected (without handshake, maybe connection loss or server crash)");

                _webSocket.Dispose();
                _webSocket = null;
                return;
            }

            // check if the web socket received a close frame
            if (result.MessageType == WebSocketMessageType.Close)
            {
                Logger?.LogWarning("Lavalink Node disconnected: {Status}, {Description}.", result.CloseStatus.Value, result.CloseStatusDescription);

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
                Logger?.LogTrace("Received payload: `{Payload}` from: {Uri}.", content, _webSocketUri);
            }

            // process data
            try
            {
                var payload = PayloadConverter.ReadPayload(content);
                await OnPayloadReceived(payload);
            }
            catch (Exception ex)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, string.Empty, CancellationToken.None);
                _webSocket.Dispose();
                _webSocket = null;

                Logger?.LogWarning(ex, "Received bad payload: {Content}.", content);
            }
        }

        /// <summary>
        ///     Runs the receive / reconnect life cycle asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        private async Task RunLifeCycleAsync()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                // receive new payload until the web-socket is connected.
                while (IsConnected && !_cancellationTokenSource.IsCancellationRequested)
                {
                    await ProcessNextPayload();
                }

                // try reconnect
                for (var attempt = 0; !_cancellationTokenSource.IsCancellationRequested; attempt++)
                {
                    // reconnect
                    await ConnectAsync();

                    // add delay between reconnects
                    if (!IsConnected && attempt > 3)
                    {
                        var delay = TimeSpan.FromSeconds(3 * Math.Min(attempt, 20));
                        Logger?.LogDebug("Waiting {Delay} before next reconnect attempt...", delay);
                        await Task.Delay(delay);
                    }
                }
            }
        }
    }
}