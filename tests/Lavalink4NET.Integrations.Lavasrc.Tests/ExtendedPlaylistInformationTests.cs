namespace Lavalink4NET.Integrations.Lavasrc.Tests;

using System.Collections.Immutable;
using System.Text.Json.Nodes;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;

public sealed class ExtendedPlaylistInformationTests
{
    [Fact]
    public void TestExtendedPlaylistInformationForwardsProperties()
    {
        // Arrange
        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: null,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty);

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.Equal(playlist.Name, extendedPlaylistInformation.Name);
        Assert.Null(extendedPlaylistInformation.SelectedTrack);
        Assert.Equal(playlist.AdditionalInformation, extendedPlaylistInformation.AdditionalInformation);
    }

    [Fact]
    public void TestExtendedPlaylistInformationWrapsTrackInExtendedLavalinkTrackIfNotNull()
    {
        // Arrange
        var track = new LavalinkTrack
        {
            Author = "author",
            Identifier = "identifier",
            Title = "title",
        };

        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: track,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty);

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.NotNull(extendedPlaylistInformation.SelectedTrack);
        Assert.Equal(track, extendedPlaylistInformation.SelectedTrack.Value.Track);
    }

    [Fact]
    public void TestPlaylistTypeIsNullIfNotPresent()
    {
        // Arrange
        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: null,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty);

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.Null(extendedPlaylistInformation.Type);
    }

    [Fact]
    public void TestPlaylistTypeIsAlbumIfPresent()
    {
        // Arrange
        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: null,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty.Add("type", "album"!));

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.Equal(PlaylistType.Album, extendedPlaylistInformation.Type);
    }

    [Fact]
    public void TestPlaylistTypeIsPlaylistIfPresent()
    {
        // Arrange
        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: null,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty.Add("type", "playlist"!));

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.Equal(PlaylistType.Playlist, extendedPlaylistInformation.Type);
    }

    [Fact]
    public void TestPlaylistTypeIsArtistIfPresent()
    {
        // Arrange
        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: null,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty.Add("type", "artist"!));

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.Equal(PlaylistType.Artist, extendedPlaylistInformation.Type);
    }

    [Fact]
    public void TestPlaylistTypeIsRecommendationsIfPresent()
    {
        // Arrange
        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: null,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty.Add("type", "recommendations"!));

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.Equal(PlaylistType.Recommendations, extendedPlaylistInformation.Type);
    }

    [Fact]
    public void TestPlaylistTypeIsNullIfNotRecognized()
    {
        // Arrange
        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: null,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty.Add("type", "not-recognized"!));

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.Null(extendedPlaylistInformation.Type);
    }

    [Fact]
    public void TestUriIsNullIfNotPresent()
    {
        // Arrange
        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: null,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty);

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.Null(extendedPlaylistInformation.Uri);
    }

    [Fact]
    public void TestUriIsPresentIfPresent()
    {
        // Arrange
        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: null,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty.Add("url", "http://url.url/"!));

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.Equal("http://url.url/", extendedPlaylistInformation.Uri?.ToString());
    }

    [Fact]
    public void TestArtworkUriIsNullIfNotPresent()
    {
        // Arrange
        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: null,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty);

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.Null(extendedPlaylistInformation.ArtworkUri);
    }

    [Fact]
    public void TestArtworkUriIsPresentIfPresent()
    {
        // Arrange
        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: null,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty.Add("artworkUrl", "http://artwork.uri/"!));

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.Equal("http://artwork.uri/", extendedPlaylistInformation.ArtworkUri?.ToString());
    }

    [Fact]
    public void TestAuthorIsNullIfNotPresent()
    {
        // Arrange
        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: null,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty);

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.Null(extendedPlaylistInformation.Author);
    }

    [Fact]
    public void TestAuthorIsPresentIfPresent()
    {
        // Arrange
        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: null,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty.Add("author", "author"!));

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.Equal("author", extendedPlaylistInformation.Author);
    }

    [Fact]
    public void TestTotalTracksIsNullIfNotPresent()
    {
        // Arrange
        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: null,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty);

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.Null(extendedPlaylistInformation.TotalTracks);
    }

    [Fact]
    public void TestTotalTracksNotNullIfPresent()
    {
        // Arrange
        var playlist = new PlaylistInformation(
            Name: "name",
            SelectedTrack: null,
            AdditionalInformation: ImmutableDictionary<string, JsonNode>.Empty.Add("totalTracks", 1));

        // Act
        var extendedPlaylistInformation = new ExtendedPlaylistInformation(playlist);

        // Assert
        Assert.Equal(1, extendedPlaylistInformation.TotalTracks);
    }
}
