/*
 *  File:   LyricsOptions.cs
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

namespace Lavalink4NET.Lyrics
{
    using Lavalink4NET.Rest;

    /// <summary>
    ///     The service options for the <see cref="LyricsService"/> class.
    /// </summary>
    public sealed class LyricsOptions : RestClientOptions
    {
        /// <summary>
        ///     Gets or sets the base endpoint of the Lyrics API service ("lyrics.ovh"). This
        ///     property can be useful when using a local lyrics.ovh API service.
        /// </summary>
        /// <remarks>
        ///     This property defaults to <c>"https://api.lyrics.ovh/v1/"</c>. Note this is an
        ///     absolute URI and can not be <see langword="null"/>.
        /// </remarks>
        public override string RestUri { get; set; } = "https://api.lyrics.ovh/v1/";

        /// <summary>
        ///     Gets or sets a value indicating whether an exception should be thrown when a response
        ///     to the lyrics.ovh API service failed (returned with a non-2xx / success HTTP status
        ///     code). (For example the lyrics.ovh API service returns with a 404 Not Found, if the
        ///     lyrics for a song were not found.)
        /// </summary>
        /// <remarks>This property defaults to <see langword="true"/>.</remarks>
        public bool SuppressExceptions { get; set; } = true;
    }
}