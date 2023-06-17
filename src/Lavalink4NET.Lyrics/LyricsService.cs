namespace Lavalink4NET.Lyrics;

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;

/// <summary>
///     A service for retrieving song lyrics from the <c>"lyrics.ovh"</c> API.
/// </summary>
public sealed class LyricsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISystemClock _systemClock;
    private readonly IMemoryCache? _memoryCache;
    private readonly TimeSpan _cacheDuration;
    private readonly bool _suppressExceptions;
    private readonly Uri _baseAddress;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LyricsService"/> class.
    /// </summary>
    /// <param name="options">the lyrics service options</param>
    /// <param name="memoryCache">the request cache</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="httpClientFactory"/> parameter is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="options"/> parameter is <see langword="null"/>.
    /// </exception>
    public LyricsService(
        IHttpClientFactory httpClientFactory,
        IOptions<LyricsOptions> options,
        ISystemClock? systemClock = null,
        IMemoryCache? memoryCache = null)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(options);

        if (options.Value.CacheDuration <= TimeSpan.Zero)
        {
            throw new InvalidOperationException(
                "The cache time is negative or zero. Please do not " +
                "specify a cache in the constructor instead of using a zero cache time.");
        }

        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;

        _systemClock = systemClock ?? new SystemClock();

        _baseAddress = options.Value.BaseAddress;
        _cacheDuration = options.Value.CacheDuration;
        _suppressExceptions = options.Value.SuppressExceptions;
    }

    /// <summary>
    ///     Gets the lyrics for a track asynchronously (cached).
    /// </summary>
    /// <param name="artist">the artist name (e.g. Coldplay)</param>
    /// <param name="title">the title of the track (e.g. "Adventure of a Lifetime")</param>
    /// <param name="cancellationToken">
    ///     a cancellation token that can be used by other objects or threads to receive notice
    ///     of cancellation.
    /// </param>
    /// <returns>
    ///     a task that represents the asynchronous operation. The task result is the track
    ///     found for the query
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="artist"/> is blank.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="title"/> is blank.
    /// </exception>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed.</exception>
    public async ValueTask<string?> GetLyricsAsync(string artist, string title, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(artist);
        ArgumentNullException.ThrowIfNull(title);

        // the cache key
        var key = $"lyrics-{artist}-{title}";

        // check if the item is cached
        if (_memoryCache != null && _memoryCache.TryGetValue<string>(key, out var item))
        {
            return item;
        }

        var response = await RequestLyricsAsync(artist, title, cancellationToken);
        _memoryCache?.Set(key, response, _systemClock.UtcNow + _cacheDuration);
        return response;
    }

    /// <summary>
    ///     Gets the lyrics for a track asynchronously (cached).
    /// </summary>
    /// <param name="track">the track information to get the lyrics for</param>
    /// <param name="cancellationToken">
    ///     a cancellation token that can be used by other objects or threads to receive notice
    ///     of cancellation.
    /// </param>
    /// <returns>
    ///     a task that represents the asynchronous operation. The task result is the lyrics
    ///     found for the query
    /// </returns>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed.</exception>
    public ValueTask<string?> GetLyricsAsync(LavalinkTrack track, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(track);

        return GetLyricsAsync(track.Author, track.Title, cancellationToken);
    }

    /// <summary>
    ///     Gets the lyrics for a track asynchronously (no caching).
    /// </summary>
    /// <param name="track">the track information to get the lyrics for</param>
    /// <param name="cancellationToken">
    ///     a cancellation token that can be used by other objects or threads to receive notice
    ///     of cancellation.
    /// </param>
    /// <returns>
    ///     a task that represents the asynchronous operation. The task result is the lyrics
    ///     found for the query
    /// </returns>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed.</exception>
    public ValueTask<string?> RequestLyricsAsync(LavalinkTrack track, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(track);

        return RequestLyricsAsync(track.Author, track.Title, cancellationToken);
    }

    /// <summary>
    ///     Gets the lyrics for a track asynchronously (no caching).
    /// </summary>
    /// <param name="artist">the artist name (e.g. Coldplay)</param>
    /// <param name="title">the title of the track (e.g. "Adventure of a Lifetime")</param>
    /// <param name="cancellationToken">
    ///     a cancellation token that can be used by other objects or threads to receive notice
    ///     of cancellation.
    /// </param>
    /// <returns>
    ///     a task that represents the asynchronous operation. The task result is the lyrics
    ///     found for the query
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="artist"/> is blank.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="title"/> is blank.
    /// </exception>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed.</exception>
    public async ValueTask<string?> RequestLyricsAsync(string artist, string title, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(artist);
        ArgumentNullException.ThrowIfNull(title);

        // encode query parameters
        title = HttpUtility.HtmlEncode(title);
        artist = HttpUtility.HtmlEncode(artist);

        // send response
        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;

        using var response = await httpClient
            .GetAsync($"{artist}/{title}", HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        var payload = await response.Content
            .ReadFromJsonAsync(LyricsJsonSerializerContext.Default.LyricsResponse, cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            // exceptions are suppressed
            if (_suppressExceptions)
            {
                return null;
            }

            throw new Exception($"Error while requesting: {response.RequestMessage?.RequestUri}\n\t\t{payload.ErrorMessage}");
        }

        return payload!.Lyrics;
    }
}
