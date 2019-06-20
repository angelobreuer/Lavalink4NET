﻿/*
 *  File:   LyricsService.cs
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

namespace Lavalink4NET.Lyrics
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Lavalink4NET.Player;
    using Newtonsoft.Json;

    /// <summary>
    ///     A service for retrieving song lyrics from the "https://lyrics.ovh" api.
    /// </summary>
    public sealed class LyricsService : IDisposable
    {
        private readonly ILavalinkCache _cache;
        private readonly TimeSpan _cacheTime;
        private readonly HttpClient _httpClient;
        private readonly bool _suppressExceptions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LyricsService"/> class.
        /// </summary>
        /// <param name="options">the lyrics service options</param>
        /// <param name="cache">the request cache</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="options"/> parameter is <see langword="null"/>.
        /// </exception>
        public LyricsService(LyricsOptions options, ILavalinkCache cache = null)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.CacheTime <= TimeSpan.Zero)
            {
                throw new InvalidOperationException("The cache time is negative or zero. Please do not " +
                    "specify a cache in the constructor instead of using a zero cache time.");
            }

            // initialize HTTP client handler
            var httpHandler = new HttpClientHandler();

            // check if automatic decompression should be used
            if (options.Decompression)
            {
                // setup compression
                httpHandler.AutomaticDecompression = DecompressionMethods.GZip
                    | DecompressionMethods.Deflate;
            }

            // disables cookies
            httpHandler.UseCookies = false;

            // initialize HTTP client
            _httpClient = new HttpClient(httpHandler)
            {
                BaseAddress = new Uri(options.RestUri)
            };

            // add user-agent request header
            if (!string.IsNullOrWhiteSpace(options.UserAgent))
            {
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", options.UserAgent);
            }

            _cache = cache;
            _cacheTime = options.CacheTime;
            _cache = cache;
            _suppressExceptions = options.SuppressExceptions;
        }

        /// <summary>
        ///     Disposes the underlying HTTP client.
        /// </summary>
        public void Dispose() => _httpClient.Dispose();

        /// <summary>
        ///     Gets the lyrics for a track asynchronously (cached).
        /// </summary>
        /// <param name="artist">the artist name (e.g. Coldplay)</param>
        /// <param name="title">the title of the track (e.g. "Adventure of a Lifetime")</param>
        /// <param name="cancellationToken">
        ///     a cancellation token that can be used by other objects or threads to receive notice
        ///     of cancellation.
        /// </param>
        /// <returns>the track found for the query</returns>
        public async Task<string> GetLyricsAsync(string artist, string title, CancellationToken cancellationToken = default)
        {
            // the cache key
            var key = $"lyrics-{artist}-{title}";

            // check if the item is cached
            if (_cache != null && _cache.TryGetItem<string>(key, out var item))
            {
                return item;
            }

            var response = await RequestLyricsAsync(artist, title, cancellationToken);
            _cache?.AddItem(key, response, DateTimeOffset.UtcNow + _cacheTime);
            return response;
        }

        /// <summary>
        ///     Gets the lyrics for a track asynchronously (cached).
        /// </summary>
        /// <param name="trackInfo">the track information to get the lyrics for</param>
        /// <param name="cancellationToken">
        ///     a cancellation token that can be used by other objects or threads to receive notice
        ///     of cancellation.
        /// </param>
        /// <returns>the lyrics found for the query</returns>
        public Task<string> GetLyricsAsync(LavalinkTrackInfo trackInfo, CancellationToken cancellationToken = default)
            => GetLyricsAsync(trackInfo.Author, trackInfo.Title, cancellationToken);

        /// <summary>
        ///     Gets the lyrics for a track asynchronously (no caching).
        /// </summary>
        /// <param name="trackInfo">the track information to get the lyrics for</param>
        /// <param name="cancellationToken">
        ///     a cancellation token that can be used by other objects or threads to receive notice
        ///     of cancellation.
        /// </param>
        /// <returns>the lyrics found for the query</returns>
        public Task<string> RequestLyricsAsync(LavalinkTrackInfo trackInfo, CancellationToken cancellationToken = default)
            => RequestLyricsAsync(trackInfo.Author, trackInfo.Title, cancellationToken);

        /// <summary>
        ///     Gets the lyrics for a track asynchronously (no caching).
        /// </summary>
        /// <param name="artist">the artist name (e.g. Coldplay)</param>
        /// <param name="title">the title of the track (e.g. "Adventure of a Lifetime")</param>
        /// <param name="cancellationToken">
        ///     a cancellation token that can be used by other objects or threads to receive notice
        ///     of cancellation.
        /// </param>
        /// <returns>the lyrics found for the query</returns>
        public async Task<string> RequestLyricsAsync(string artist, string title, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // encode query parameters
            title = HttpUtility.HtmlEncode(title);
            artist = HttpUtility.HtmlEncode(artist);

            // send response
            using (var response = await _httpClient.GetAsync($"{artist}/{title}", cancellationToken))
            {
                var content = await response.Content.ReadAsStringAsync();
                var payload = JsonConvert.DeserializeObject<LyricsResponse>(content);

                if (!response.IsSuccessStatusCode)
                {
                    // exceptions are suppressed
                    if (_suppressExceptions)
                    {
                        return null;
                    }

                    throw new Exception($"Error while requesting: {response.RequestMessage.RequestUri}\n\t\t{payload.ErrorMessage}");
                }

                return payload.Lyrics;
            }
        }
    }
}