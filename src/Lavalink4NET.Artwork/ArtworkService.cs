namespace Lavalink4NET.Artwork;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Tracks;

public class ArtworkService : IArtworkService, IDisposable
{
    private bool _disposed;
    private HttpClient? _httpClient;

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

    protected virtual ValueTask<Uri?> ResolveArtworkForSoundCloudAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDisposed();
        return new ValueTask<Uri?>(lavalinkTrack.ArtworkUri);
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
}
