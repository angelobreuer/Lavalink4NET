/*
 *  File:   LavalinkNodeOptions.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2020
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
    using Rest;

    /// <summary>
    ///     The required options used to connect to a lavalink node.
    /// </summary>
    public sealed class LavalinkNodeOptions : LavalinkRestOptions
    {
        /// <summary>
        ///     Gets or sets a value indicating whether session resuming should be used when the
        ///     connection to the node is aborted.
        /// </summary>
        /// <remarks>This property defaults to <see langword="true"/>.</remarks>
        public bool AllowResuming { get; set; } = true;

        /// <summary>
        ///     Gets or sets the buffer size when receiving payloads from a lavalink node.
        /// </summary>
        /// <remarks>This property defaults to <c>1024</c> (1 KiB)</remarks>
        public int BufferSize { get; set; } = 1024;

        /// <summary>
        ///     Gets or sets a value indicating whether the player should disconnect from the voice
        ///     channel its connected to after the track ended.
        /// </summary>
        /// <remarks>
        ///     This property defaults to <see langword="true"/>. This can be useful to set to <see
        ///     langword="false"/>, for example when using the InactivityTrackingService.
        /// </remarks>
        public bool DisconnectOnStop { get; set; } = true;

        /// <summary>
        ///     Gets or sets the node label.
        /// </summary>
        /// <remarks>
        ///     This property defaults to <see langword="null"/> and is used for identifying nodes.
        /// </remarks>
        public string? Label { get; set; }

        /// <summary>
        ///     Gets or sets the reconnect strategy for reconnection.
        /// </summary>
        /// <remarks>This property defaults to <see cref="ReconnectStrategies.DefaultStrategy"/>.</remarks>
        public ReconnectStrategy ReconnectStrategy { get; set; } = ReconnectStrategies.DefaultStrategy;

        /// <summary>
        ///     The number of seconds a session is valid after the connection aborts.
        /// </summary>
        /// <remarks>This property defaults to <c>60</c>.</remarks>
        public int SessionTimeout { get; set; } = 60;

        /// <summary>
        ///     Gets or sets the Lavalink Node WebSocket host(name).
        /// </summary>
        /// <remarks>This property defaults to <c>ws://localhost:8080/</c>.</remarks>
        public string WebSocketUri { get; set; } = "ws://localhost:8080/";
    }
}
