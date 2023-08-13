namespace Lavalink4NET.Lyrics;

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Lyrics.Models;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
///     A service for retrieving song lyrics from the <c>"lyrics.ovh"</c> API.
/// </summary>
public sealed class LyricsService : ILyricsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LyricsService> _logger;
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
        ILogger<LyricsService> logger,
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
        _logger = logger;
        _memoryCache = memoryCache;

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
        async Task<string?> GetOrCreateAsync(ICacheEntry entry)
        {
            var task = RequestLyricsAsync(artist, title, CancellationToken.None);
            var response = await task.ConfigureAwait(false);

            entry.Value = response;
            entry.Size = response?.Length ?? 0;
            entry.AbsoluteExpirationRelativeToNow = _cacheDuration;

            return response;
        }

        var memoryCache = _memoryCache;

        if (memoryCache is null)
        {
            return await RequestLyricsAsync(artist, title, cancellationToken).ConfigureAwait(false);
        }

        return await memoryCache
            .GetOrCreateAsync($"@LYRICS@{artist}@{title}@", GetOrCreateAsync)
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
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

    private async ValueTask<string?> RequestLyricsAsync(string artist, string title, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(artist);
        ArgumentNullException.ThrowIfNull(title);

        // encode query parameters
        title = Uri.EscapeDataString(title);
        artist = Uri.EscapeDataString(artist);

        // send response
        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;

        using var response = await httpClient
            .GetAsync($"{artist}/{title}", HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        var lyricsResponse = default(LyricsResponse?);
        try
        {
            lyricsResponse = await response.Content
                .ReadFromJsonAsync(LyricsJsonSerializerContext.Default.LyricsResponse, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (JsonException)
        {
            // ignore
        }

        if (!response.IsSuccessStatusCode)
        {
            var stringBuilder = new StringBuilder("Error while requesting: ");
            stringBuilder.AppendLine(response.RequestMessage!.RequestUri!.ToString());

            if (lyricsResponse?.ErrorMessage is not null)
            {
                stringBuilder.AppendLine(lyricsResponse.ErrorMessage);
            }

            var exception = new HttpRequestException(stringBuilder.ToString());

            // exceptions are suppressed
            if (_suppressExceptions)
            {
                _logger.ErrorWhileLoadingLyrics(artist, title);
                return null;
            }

            throw exception;
        }

        return lyricsResponse!.Lyrics;
    }
}

internal static partial class Logging
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "Error while loading lyrics for artist '{Artist}' and track '{Track}'.", EventName = nameof(ErrorWhileLoadingLyrics))]
    public static partial void ErrorWhileLoadingLyrics(this ILogger<LyricsService> logger, string artist, string track);
}