namespace Lavalink4NET.Integrations.Lavasrc;

using System.Collections.Immutable;
using System.Text.Json.Nodes;
using Lavalink4NET.Tracks;

public readonly record struct ExtendedLavalinkTrack(LavalinkTrack Track)
{
    public string Title => Track.Title;

    public string Identifier => Track.Identifier;

    public string Author => Track.Author;

    public TimeSpan Duration => Track.Duration;

    public bool IsLiveStream => Track.IsLiveStream;

    public bool IsSeekable => Track.IsSeekable;

    public Uri? Uri => Track.Uri;

    public Uri? ArtworkUri => Track.ArtworkUri;

    public string? Isrc => Track.Isrc;

    public string? SourceName => Track.SourceName;

    public TimeSpan? StartPosition => Track.StartPosition;

    public string? ProbeInfo => Track.ProbeInfo;

    public StreamProvider? Provider => Track.Provider;

    public IImmutableDictionary<string, JsonNode> AdditionalInformation => Track.AdditionalInformation;

    public ValueTask<LavalinkTrack> GetPlayableTrackAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Track.GetPlayableTrackAsync(cancellationToken);
    }

    public TrackAlbum? Album
    {
        get
        {
            var albumName = Track.AdditionalInformation.GetValueOrDefault("albumName")?.ToString();
            var albumUriValue = Track.AdditionalInformation.GetValueOrDefault("albumUrl")?.ToString();

            if (albumName is null)
            {
                // Album name is required
                return null;
            }

            var albumUri = albumUriValue is null
                ? null
                : new Uri(albumUriValue);

            return new TrackAlbum(albumName, albumUri);
        }
    }

    public TrackArtist? Artist
    {
        get
        {
            var artistUriValue = Track.AdditionalInformation.GetValueOrDefault("artistUrl")?.ToString();
            var artistArtworkUriValue = Track.AdditionalInformation.GetValueOrDefault("artistArtworkUrl")?.ToString();

            if (artistUriValue is null && artistArtworkUriValue is null)
            {
                return null;
            }

            var artistUri = artistUriValue is null
                ? null
                : new Uri(artistUriValue);

            var artistArtworkUri = artistArtworkUriValue is null
                ? null
                : new Uri(artistArtworkUriValue);

            return new TrackArtist(artistUri, artistArtworkUri);
        }
    }

    public bool IsPreview
    {
        get => Track.AdditionalInformation.GetValueOrDefault("isPreview")?.GetValue<bool?>() ?? false;
    }

    public Uri? PreviewUri
    {
        get
        {
            var previewUriValue = Track.AdditionalInformation.GetValueOrDefault("previewUrl")?.ToString();

            if (previewUriValue is null)
            {
                return null;
            }

            return new Uri(previewUriValue);
        }
    }
}
