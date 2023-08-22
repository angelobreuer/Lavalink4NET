namespace Lavalink4NET.Integrations.Lavasrc;

using System.Collections.Immutable;
using System.Text.Json;
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

    public IImmutableDictionary<string, JsonElement> AdditionalInformation => Track.AdditionalInformation;

    public ValueTask<LavalinkTrack> GetPlayableTrackAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Track.GetPlayableTrackAsync(cancellationToken);
    }

    public TrackAlbum? Album
    {
        get
        {
            if (!TryGetElement("albumName", out var albumNameElement))
            {
                // Album name is required
                return null;
            }

            var albumUri = !TryGetElement("albumUrl", out var albumUrlElement)
                ? null
                : new Uri(albumUrlElement.GetString()!);

            return new TrackAlbum(albumNameElement.GetString()!, albumUri);
        }
    }

    public TrackArtist? Artist
    {
        get
        {
            var artistUri = !TryGetElement("artistUrl", out var artistUriElement)
                ? null
                : new Uri(artistUriElement.GetString()!);

            var artistArtworkUri = !TryGetElement("artistArtworkUrl", out var artistArtworkUriElement)
                ? null
                : new Uri(artistArtworkUriElement.GetString()!);

            if (artistUri is null && artistArtworkUri is null)
            {
                return null;
            }

            return new TrackArtist(artistUri, artistArtworkUri);
        }
    }

    public bool IsPreview
    {
        get => TryGetElement("isPreview", out var previewElement) && previewElement.ValueKind is JsonValueKind.True;
    }

    public Uri? PreviewUri
    {
        get
        {
            if (!TryGetElement("previewUrl", out var previewUriElement))
            {
                return null;
            }

            return new Uri(previewUriElement.GetString()!);
        }
    }

    private bool TryGetElement(string key, out JsonElement element)
    {
        return Track.AdditionalInformation.TryGetValue(key, out element) && element.ValueKind is not JsonValueKind.Null;
    }
}
