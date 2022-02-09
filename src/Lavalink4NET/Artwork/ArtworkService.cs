namespace Lavalink4NET.Artwork;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Player;

public class ArtworkService : IArtworkService
{
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

    protected virtual ValueTask<Uri?> ResolveArtworkForCustomTrackAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        return ForwardToNonImplementedResolverAsync(lavalinkTrack, cancellationToken);
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForYouTubeAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var uri = new Uri($"https://img.youtube.com/vi/{lavalinkTrack.TrackIdentifier}/maxresdefault.jpg");
        return CreateResultFromSynchronous(uri);
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForSoundCloudAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ForwardToNonImplementedResolverAsync(lavalinkTrack, cancellationToken);
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForBandcampAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ForwardToNonImplementedResolverAsync(lavalinkTrack, cancellationToken);
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForVimeoAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var uri = new Uri($"https://i.vimeocdn.com/video/{lavalinkTrack.TrackIdentifier}.png");
        return CreateResultFromSynchronous(uri);
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForTwitchAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ForwardToNonImplementedResolverAsync(lavalinkTrack, cancellationToken);
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForLocalAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ForwardToNonImplementedResolverAsync(lavalinkTrack, cancellationToken);
    }

    protected virtual ValueTask<Uri?> ResolveArtworkForHttpAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ForwardToNonImplementedResolverAsync(lavalinkTrack, cancellationToken);
    }

    private static ValueTask<Uri?> ForwardToNonImplementedResolverAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
#if NET5_0_OR_GREATER
        return ValueTask.FromResult<Uri?>(null);
#else
        return new ValueTask<Uri?>(Task.FromResult<Uri?>(null));
#endif
    }

    private static ValueTask<Uri?> CreateResultFromSynchronous(Uri? resultUri = null)
    {
#if NET5_0_OR_GREATER
        return ValueTask.FromResult<Uri?>(resultUri);
#else
        return new ValueTask<Uri?>(Task.FromResult<Uri?>(resultUri));
#endif
    }
}
