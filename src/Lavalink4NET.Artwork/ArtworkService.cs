namespace Lavalink4NET.Artwork;

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;

public class ArtworkService : IArtworkService, IDisposable
{
    private readonly ISystemClock _systemClock;
    private readonly IMemoryCache? _cache;
    private bool _disposed;
    private HttpClient? _httpClient;

    public ArtworkService(ISystemClock? systemClock = null, IMemoryCache? cache = null)
    {
        _systemClock = systemClock ?? new SystemClock();
        _cache = cache;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public virtual ValueTask<Uri?> ResolveAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default) => lavalinkTrack.Provider switch
    {
        StreamProvider.YouTube => ResolveArtworkForYouTubeAsync(lavalinkTrack, cancellationToken),
        StreamProvider.SoundCloud => ResolveArtworkForSoundCloudAsync(lavalinkTrack, cancellationToken),
        StreamProvider.Bandcamp => ResolveArtworkForBandcampAsync(lavalinkTrack, cancellationToken),
        StreamProvider.Vimeo => ResolveArtworkForVimeoAsync(lavalinkTrack, cancellationToken),
        StreamProvider.Twitch => ResolveArtworkForTwitchAsync(lavalinkTrack, cancellationToken),
        StreamProvider.Local => ResolveArtworkForLocalAsync(lavalinkTrack, cancellationToken),
        StreamProvider.Http => ResolveArtworkForHttpAsync(lavalinkTrack, cancellationToken),
        _ => ResolveArtworkForCustomTrackAsync(lavalinkTrack, cancellationToken),
    };


    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (disposing)
        {
            _httpClient?.Dispose();
        }
    }

    protected HttpClient GetHttpClient()
    {
        EnsureNotDisposed();
        return _httpClient ??= new HttpClient();
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForBandcampAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();
        return default;
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForCustomTrackAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();
        return default;
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForHttpAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();
        return default;
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForLocalAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();
        return default;
    }

    protected virtual async ValueTask<Uri?> ResolveArtworkForSoundCloudAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();

        var cacheKey = default(string?);
        if (_cache is not null)
        {
            cacheKey = $"soundcloud-artwork-{lavalinkTrack.Identifier}";

            if (_cache.TryGetValue<Uri>(cacheKey, out var cacheItem))
            {
                return cacheItem;
            }
        }

        if (lavalinkTrack.Uri is null)
        {
            return null;
        }

        var requestUri = new Uri($"https://soundcloud.com/oembed?url={lavalinkTrack.Uri.AbsoluteUri}&format=json");
        Uri? thumbnailUri;
        try
        {
            thumbnailUri = await FetchTrackUriFromOEmbedCompatibleResourceAsync(requestUri, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException)
        {
            return null;
        }

        _cache?.Set(cacheKey!, thumbnailUri, _systemClock.UtcNow + TimeSpan.FromMinutes(60));

        return thumbnailUri;
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForTwitchAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();
        return default;
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForVimeoAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();

        var uri = new Uri($"https://i.vimeocdn.com/video/{lavalinkTrack.Identifier}.png");
        return ValueTask.FromResult<Uri?>(uri);
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForYouTubeAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();

        var uri = new Uri($"https://img.youtube.com/vi/{lavalinkTrack.Identifier}/maxresdefault.jpg");
        return ValueTask.FromResult<Uri?>(uri);
    }

    private void EnsureNotDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ArtworkService));
        }
    }

    private async ValueTask<Uri?> FetchTrackUriFromOEmbedCompatibleResourceAsync(Uri requestUri, CancellationToken cancellationToken = default)
    {
        var httpResponse = await GetHttpClient()
            .GetFromJsonAsync<JsonObject>(requestUri, cancellationToken)
            .ConfigureAwait(false);

        // OEmbed responses contain a thumbnail_uri property as per standard
        var thumbnailUriNode = httpResponse?["thumbnail_url"]
            ?? throw new InvalidOperationException("Unable to find thumbnail URI in response.");

        var rawThumbnailUri = thumbnailUriNode.GetValue<string>();
        return rawThumbnailUri is null ? default : new Uri(rawThumbnailUri);
    }
}
