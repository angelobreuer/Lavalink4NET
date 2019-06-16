/*
 *  File:   LavalinkRestClient.cs
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

namespace Lavalink4NET.Rest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Player;

    /// <summary>
    ///     Lavalink RESTful HTTP api client.
    /// </summary>
    public class LavalinkRestClient : ILavalinkRestClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<Lavalink> _logger;
        private readonly ILavalinkCache _cache;
        private readonly TimeSpan _cacheTime;
        private readonly bool _debugPayloads;

        /// <summary>
        ///     The header name for the version of the Lavalink HTTP response from the node. See
        ///     https://github.com/Frederikam/Lavalink/blob/master/IMPLEMENTATION.md#significant-changes-v20---v30
        ///     for more details.
        /// </summary>
        private const string VersionHeaderName = "Lavalink-Api-Version";

        /// <summary>
        ///     Initializes a new instance of the <see cref="LavalinkRestClient"/> class.
        /// </summary>
        /// <param name="options">the rest client options</param>
        /// <param name="logger">the logger</param>
        /// <param name="cache">an optional cache that caches track requests</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="options"/> parameter is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the track cache time ( <see cref="LavalinkRestOptions.TrackCacheTime"/>) is
        ///     equal or less than <see cref="TimeSpan.Zero"/>.
        /// </exception>
        public LavalinkRestClient(LavalinkRestOptions options, ILogger<Lavalink> logger = null, ILavalinkCache cache = null)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.TrackCacheTime <= TimeSpan.Zero)
            {
                throw new InvalidOperationException("The track cache time is negative or zero. Please do not " +
                    "specify a cache in the constructor instead of using a zero cache time.");
            }

            _httpClient = new HttpClient { BaseAddress = new Uri(options.RestUri) };
            _httpClient.DefaultRequestHeaders.Add("Authorization", options.Password);

            _logger = logger;
            _cache = cache;
            _cacheTime = options.TrackCacheTime;
            _debugPayloads = options.DebugPayloads;
        }

        /// <summary>
        ///     Verifies the specified <paramref name="response"/>. This makes sure that the right
        ///     Lavalink Server version is used and the response status code is success.
        /// </summary>
        /// <param name="response">the response received</param>
        private void VerifyResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new InvalidOperationException("Received 403 Forbidden response from Lavalink node. Make sure you are using the right password.");
            }

            response.EnsureSuccessStatusCode();

            if (!response.Headers.TryGetValues(VersionHeaderName, out var values))
            {
                _logger?.LogWarning("Expected header `{Header}` on Lavalink HTTP response. Make sure you're using the Lavalink-Server >= 3.0", VersionHeaderName);
                return;
            }

            var rawVersion = values.Single();

            if (!int.TryParse(rawVersion, out var version) || version <= 2)
            {
                _logger?.LogWarning("Invalid version {Version}, supported version >= 3. Make sure you're using the Lavalink-Server >= 3.0", version);
            }
        }

        /// <summary>
        ///     Disposes the inner HTTP client.
        /// </summary>
        public virtual void Dispose() => _httpClient.Dispose();

        /// <summary>
        ///     Gets the track for the specified <paramref name="query"/> asynchronously.
        /// </summary>
        /// <param name="query">the track search query</param>
        /// <param name="mode">the track search mode</param>
        /// <param name="noCache">
        ///     a value indicating whether the track should be returned from cache, if it is cached.
        ///     Note this parameter does only take any effect is a cache provider is specified in constructor.
        /// </param>
        /// <returns>the track found for the query</returns>
        public async Task<LavalinkTrack> GetTrackAsync(string query, SearchMode mode = SearchMode.None, bool noCache = false)
            => (await GetTracksAsync(query, mode, noCache))?.FirstOrDefault();

        /// <summary>
        ///     Gets the tracks for the specified <paramref name="query"/> asynchronously.
        /// </summary>
        /// <param name="query">the track search query</param>
        /// <param name="mode">the track search mode</param>
        /// <param name="noCache">
        ///     a value indicating whether the track should be returned from cache, if it is cached.
        ///     Note this parameter does only take any effect is a cache provider is specified in constructor.
        /// </param>
        /// <returns>the tracks found for the query</returns>
        public async Task<IEnumerable<LavalinkTrack>> GetTracksAsync(string query, SearchMode mode = SearchMode.None, bool noCache = false)
            => (await LoadTracksAsync(query, mode, noCache))?.Tracks;

        /// <summary>
        ///     Loads the tracks specified by the <paramref name="query"/> asynchronously.
        /// </summary>
        /// <param name="query">the search query</param>
        /// <param name="mode">the track search mode</param>
        /// <param name="noCache">
        ///     a value indicating whether the track should be returned from cache, if it is cached.
        ///     Note this parameter does only take any effect is a cache provider is specified in constructor.
        /// </param>
        /// <returns>
        ///     a task that represents the asynchronous operation <param>the request response</param>
        /// </returns>
        public async Task<TrackLoadResponsePayload> LoadTracksAsync(string query, SearchMode mode = SearchMode.None, bool noCache = false)
        {
            if (!(query.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase)
                && query.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (mode == SearchMode.SoundCloud)
                {
                    query = "scsearch:" + query;
                }
                else if (mode == SearchMode.YouTube)
                {
                    query = "ytsearch:" + query;
                }
            }

            // check if a cache provider is specified in constructor and the track request is cached
            // and caching is wanted (see: "noCache" method parameter)
            if (_cache != null && !noCache && _cache.TryGetItem<TrackLoadResponsePayload>("track-" + query, out var track))
            {
                _logger?.LogDebug("Loaded track from cache `{Query}`.", query);
                return track;
            }

            _logger?.LogDebug("Loading track '{Query}'...", query);

            using (var response = await _httpClient.GetAsync($"loadtracks?identifier={query}"))
            {
                VerifyResponse(response);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (_debugPayloads)
                {
                    _logger?.LogDebug("Got response for track load: `{Query}`: {Payload}.", query, responseContent);
                }

                var trackLoad = JsonConvert.DeserializeObject<TrackLoadResponsePayload>(responseContent);

                // cache (if a cache provider is specified)
                _cache?.AddItem("track-" + query, trackLoad, DateTimeOffset.UtcNow + _cacheTime);

                return trackLoad;
            }
        }
    }
}