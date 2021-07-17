/*
 *  File:   RestClientOptions.cs
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

namespace Lavalink4NET.Rest
{
    using System;

    /// <summary>
    ///     An abstraction for the options for a RESTful HTTP client.
    /// </summary>
    public abstract class RestClientOptions
    {
        /// <summary>
        ///     Gets or sets the time how long a request should be cached.
        /// </summary>
        /// <remarks>
        ///     Note higher time spans can cause more memory usage, but reduce the number of requests made.
        /// </remarks>
        /// <remarks>This property defaults to <c>TimeSpan.FromMinutes(5)</c>.</remarks>
        public TimeSpan CacheTime { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        ///     Gets or sets a value indicating whether the HTTP client accepts compressed payloads.
        /// </summary>
        /// <remarks>This property defaults to <see langword="true"/>.</remarks>
        public bool Decompression { get; set; } = true;

        /// <summary>
        ///     Gets or sets the RESTful HTTP api endpoint.
        /// </summary>
        /// <remarks>This property defaults to <c>http://localhost:8080/</c>.</remarks>
        public abstract string RestUri { get; set; }

        /// <summary>
        ///     Gets or sets the user-agent for HTTP requests (use <see langword="null"/> to disable
        ///     the custom user-agent header).
        /// </summary>
        /// <remarks>This property defaults to <c>"Lavalink4NET"</c>.</remarks>
        public string UserAgent { get; set; } = "Lavalink4NET";
    }
}