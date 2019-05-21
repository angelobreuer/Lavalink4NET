/* 
 *  File:   LavalinkNodeOptions.cs
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
    using Rest;

    /// <summary>
    ///     The required options used to connect to a lavalink node.
    /// </summary>
    public sealed class LavalinkNodeOptions : LavalinkRestOptions
    {
        /// <summary>
        ///     Gets or sets the Lavalink Node WebSocket host(name).
        /// </summary>
        public string WebSocketUri { get; set; } = "ws://localhost:8080/";

        /// <summary>
        ///     Gets or sets the buffer size when receiving payloads from a lavalink node.
        /// </summary>
        public int BufferSize { get; set; } = 1024 * 1024;

        /// <summary>
        ///     Gets or sets a value indicating whether payload I/O should be logged to the logger
        ///     (This should be only used for development)
        /// </summary>
        public bool DebugPayloads { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether session resuming should be used when the
        ///     connection to the node is aborted.
        /// </summary>
        public bool AllowResuming { get; set; } = true;
    }
}