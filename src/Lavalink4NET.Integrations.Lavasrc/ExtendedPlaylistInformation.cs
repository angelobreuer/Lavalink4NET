namespace Lavalink4NET.Integrations.Lavasrc;

using System.Collections.Immutable;
using System.Text.Json.Nodes;
using Lavalink4NET.Rest.Entities.Tracks;

public readonly record struct ExtendedPlaylistInformation(PlaylistInformation Playlist)
{
    public string Name => Playlist.Name;

    public ExtendedLavalinkTrack? SelectedTrack
    {
        get
        {
            return Playlist.SelectedTrack is null
                ? null
                : new ExtendedLavalinkTrack(Playlist.SelectedTrack);
        }
    }

    public IImmutableDictionary<string, JsonNode> AdditionalInformation => Playlist.AdditionalInformation;

    public PlaylistType? Type => AdditionalInformation.GetValueOrDefault("type")?.ToString() switch
    {
        "album" => PlaylistType.Album,
        "playlist" => PlaylistType.Playlist,
        "artist" => PlaylistType.Artist,
        "recommendations" => PlaylistType.Recommendations,
        _ => null,
    };

    public Uri? Uri
    {
        get
        {
            var uri = AdditionalInformation["url"]?.ToString();
            return uri is null ? null : new Uri(uri);
        }
    }

    public Uri? ArtworkUri
    {
        get
        {
            var artworkUri = AdditionalInformation["artworkUrl"]?.ToString();
            return artworkUri is null ? null : new Uri(artworkUri);
        }
    }

    public string? Author => AdditionalInformation["author"]?.ToString();

    public int? TotalTracks => AdditionalInformation["totalTracks"]?.GetValue<int>();
}
