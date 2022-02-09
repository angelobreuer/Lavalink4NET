namespace Lavalink4NET.Artwork;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Player;

public class ArtworkService : IArtworkService, IDisposable
{
    private readonly ILavalinkCache? _cache;
    private HttpClient? _httpClient;
    private bool _disposed;

    public ArtworkService(ILavalinkCache? cache = null)
    {
        _cache = cache;
    }

    /// <inheritdoc/>
    public ValueTask<Uri?> ResolveAsync(string trackId, StreamProvider streamProvider, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        return ResolveInternalAsync(trackId, lavalinkTrack: null, streamProvider, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual ValueTask<Uri?> ResolveAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        return ResolveInternalAsync(lavalinkTrack.TrackIdentifier, lavalinkTrack, lavalinkTrack.Provider, cancellationToken);
    }

    private ValueTask<Uri?> ResolveInternalAsync(string trackId, LavalinkTrack? lavalinkTrack, StreamProvider streamProvider, CancellationToken cancellationToken = default) => streamProvider switch
    {
        StreamProvider.YouTube => ResolveArtworkForYouTubeAsync(trackId, lavalinkTrack, cancellationToken),
        StreamProvider.SoundCloud => ResolveArtworkForSoundCloudAsync(trackId, lavalinkTrack, cancellationToken),
        StreamProvider.Bandcamp => ResolveArtworkForBandcampAsync(trackId, lavalinkTrack, cancellationToken),
        StreamProvider.Vimeo => ResolveArtworkForVimeoAsync(trackId, lavalinkTrack, cancellationToken),
        StreamProvider.Twitch => ResolveArtworkForTwitchAsync(trackId, lavalinkTrack, cancellationToken),
        StreamProvider.Local => ResolveArtworkForLocalAsync(trackId, lavalinkTrack, cancellationToken),
        StreamProvider.Http => ResolveArtworkForHttpAsync(trackId, lavalinkTrack, cancellationToken),
        _ => ResolveArtworkForCustomTrackAsync(trackId, lavalinkTrack, cancellationToken),
    };

    protected virtual ValueTask<Uri?> ResolveArtworkForCustomTrackAsync(string trackId, LavalinkTrack? lavalinkTrack = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();
        return default;
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForYouTubeAsync(string trackId, LavalinkTrack? lavalinkTrack = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();

        var uri = new Uri($"https://img.youtube.com/vi/{trackId}/maxresdefault.jpg");
        return CreateResultFromSynchronous(uri);
    }

    protected virtual async ValueTask<Uri?> ResolveArtworkForSoundCloudAsync(string trackId, LavalinkTrack? lavalinkTrack = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();
        return default;
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForBandcampAsync(string trackId, LavalinkTrack? lavalinkTrack = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();
        return default;
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForVimeoAsync(string trackId, LavalinkTrack? lavalinkTrack = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();

        var uri = new Uri($"https://i.vimeocdn.com/video/{trackId}.png");
        return CreateResultFromSynchronous(uri);
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForTwitchAsync(string trackId, LavalinkTrack? lavalinkTrack = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();
        return default;
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForLocalAsync(string trackId, LavalinkTrack? lavalinkTrack = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();
        return default;
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForHttpAsync(string trackId, LavalinkTrack? lavalinkTrack = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();
        return default;
    }

    private static ValueTask<Uri?> CreateResultFromSynchronous(Uri? resultUri = null)
    {
#if NET5_0_OR_GREATER
        return ValueTask.FromResult(resultUri);
#else
        return new ValueTask<Uri?>(Task.FromResult<Uri?>(resultUri));
#endif
    }

    protected HttpClient GetHttpClient()
    {
        EnsureNotDisposed();
        return _httpClient ??= new HttpClient();
    }

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

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void EnsureNotDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ArtworkService));
        }
    }
}
