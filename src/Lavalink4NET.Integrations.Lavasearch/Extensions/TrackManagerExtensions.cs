namespace Lavalink4NET.Integrations.Lavasearch.Extensions;

using System.Collections.Immutable;
using System.Threading.Tasks;
using Lavalink4NET.Protocol.Responses;
using Lavalink4NET.Rest;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;

public static class TrackManagerExtensions
{
    public static async ValueTask<SearchResult?> SearchAsync(
       this ITrackManager trackManager,
       string query,
       ImmutableArray<SearchCategory>? categories = null,
       TrackLoadOptions loadOptions = default,
       LavalinkApiResolutionScope resolutionScope = default,
       CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(trackManager);
        ArgumentNullException.ThrowIfNull(query);

        var apiClient = await resolutionScope
            .GetClientAsync(trackManager.ApiClientProvider, cancellationToken)
            .ConfigureAwait(false);

        var searchResult = await apiClient
            .SearchAsync(query, categories, loadOptions, cancellationToken)
            .ConfigureAwait(false);

        if (searchResult is null)
        {
            return null;
        }

        static PlaylistInformation CreatePlaylist(PlaylistLoadResultData data)
        {
            var playlistName = data.PlaylistInformation.Name;

            return new PlaylistInformation(
                Name: playlistName,
                SelectedTrack: null,
                AdditionalInformation: data.AdditionalInformation);
        }

        var tracks = searchResult.Tracks is null
            ? ImmutableArray<LavalinkTrack>.Empty
            : searchResult.Tracks.Value.Select(LavalinkApiClient.CreateTrack).ToImmutableArray();

        var albums = searchResult.Albums is null
            ? ImmutableArray<PlaylistInformation>.Empty
            : searchResult.Albums.Value.Select(CreatePlaylist).ToImmutableArray();

        var artists = searchResult.Artists is null
            ? ImmutableArray<PlaylistInformation>.Empty
            : searchResult.Artists.Value.Select(CreatePlaylist).ToImmutableArray();

        var playlists = searchResult.Playlists is null
            ? ImmutableArray<PlaylistInformation>.Empty
            : searchResult.Playlists.Value.Select(CreatePlaylist).ToImmutableArray();

        var texts = searchResult.Texts is null
            ? ImmutableArray<TextResult>.Empty
            : searchResult.Texts.Value.Select(x => new TextResult(x.Text, x.AdditionalInformation)).ToImmutableArray();

        return new SearchResult(
            Tracks: tracks,
            Albums: albums,
            Artists: artists,
            Playlists: playlists,
            Texts: texts,
            AdditionalInformation: searchResult.AdditionalInformation);
    }
}
