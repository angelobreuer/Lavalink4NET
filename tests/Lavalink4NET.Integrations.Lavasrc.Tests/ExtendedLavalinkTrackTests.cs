namespace Lavalink4NET.Integrations.Lavasrc.Tests;

using System.Collections.Immutable;
using System.Text.Json;
using Lavalink4NET.Tracks;

public class ExtendedLavalinkTrackTests
{
    [Fact]
    public void TestPropertiesForwardedProperly()
    {
        // Arrange
        var track = new LavalinkTrack
        {
            Author = "author",
            ArtworkUri = new Uri("https://artwork.uri"),
            Duration = TimeSpan.FromSeconds(1),
            Identifier = "identifier",
            IsLiveStream = true,
            IsSeekable = true,
            Title = "title",
            Isrc = "isrc",
            SourceName = "YouTube",
            StartPosition = TimeSpan.FromSeconds(2),
            Uri = new Uri("https://uri/"),
            ProbeInfo = "abc",
            AdditionalInformation = ImmutableDictionary<string, JsonElement>.Empty,
        };

        var extendedTrack = new ExtendedLavalinkTrack(track);

        // Assert
        Assert.Equal(track.Author, extendedTrack.Author);
        Assert.Equal(track.ArtworkUri, extendedTrack.ArtworkUri);
        Assert.Equal(track.Duration, extendedTrack.Duration);
        Assert.Equal(track.Identifier, extendedTrack.Identifier);
        Assert.Equal(track.IsLiveStream, extendedTrack.IsLiveStream);
        Assert.Equal(track.IsSeekable, extendedTrack.IsSeekable);
        Assert.Equal(track.Title, extendedTrack.Title);
        Assert.Equal(track.Isrc, extendedTrack.Isrc);
        Assert.Equal(track.SourceName, extendedTrack.SourceName);
        Assert.Equal(track.StartPosition, extendedTrack.StartPosition);
        Assert.Equal(track.Uri, extendedTrack.Uri);
        Assert.Equal(track.ProbeInfo, extendedTrack.ProbeInfo);
        Assert.Equal(StreamProvider.YouTube, extendedTrack.Provider);
        Assert.Equal(track.AdditionalInformation, extendedTrack.AdditionalInformation);
    }

    [Fact]
    public void TestGetAlbum()
    {
        // Arrange
        var track = new LavalinkTrack
        {
            Title = "title",
            Author = "author",
            Identifier = "identifier",

            AdditionalInformation = new Dictionary<string, JsonElement>
            {
                ["albumName"] = JsonSerializer.SerializeToElement("albumName"),
                ["albumUrl"] = JsonSerializer.SerializeToElement("https://album.uri/"),
            }.ToImmutableDictionary(),
        };

        var extendedTrack = new ExtendedLavalinkTrack(track);

        // Act
        var album = extendedTrack.Album;

        // Assert
        Assert.NotNull(album);
        Assert.Equal("albumName", album.Value.Name);
        Assert.Equal(new Uri("https://album.uri"), album.Value.Uri);
    }

    [Fact]
    public void TestGetAlbumIsNullIfNameMissing()
    {
        // Arrange
        var track = new LavalinkTrack
        {
            Title = "title",
            Author = "author",
            Identifier = "identifier",

            AdditionalInformation = new Dictionary<string, JsonElement>
            {
                ["albumUrl"] = JsonSerializer.SerializeToElement("https://album.uri/"),
            }.ToImmutableDictionary(),
        };

        var extendedTrack = new ExtendedLavalinkTrack(track);

        // Act
        var album = extendedTrack.Album;

        // Assert
        Assert.Null(album);
    }

    [Fact]
    public void TestGetAlbumNotNullIfNamePresentButUriMissing()
    {
        // Arrange
        var track = new LavalinkTrack
        {
            Title = "title",
            Author = "author",
            Identifier = "identifier",

            AdditionalInformation = new Dictionary<string, JsonElement>
            {
                ["albumName"] = JsonSerializer.SerializeToElement("albumName"),
            }.ToImmutableDictionary(),
        };

        var extendedTrack = new ExtendedLavalinkTrack(track);

        // Act
        var album = extendedTrack.Album;

        // Assert
        Assert.NotNull(album);
        Assert.Equal("albumName", album.Value.Name);
        Assert.Null(album.Value.Uri);
    }

    [Fact]
    public void TestArtistNullIfArtistUriAndArtistArtworkUriMissing()
    {
        // Arrange
        var track = new LavalinkTrack
        {
            Title = "title",
            Author = "author",
            Identifier = "identifier",
        };

        var extendedTrack = new ExtendedLavalinkTrack(track);

        // Act
        var artist = extendedTrack.Artist;

        // Assert
        Assert.Null(artist);
    }

    [Fact]
    public void TestArtistPresentIfArtistUriPresent()
    {
        // Arrange
        var track = new LavalinkTrack
        {
            Title = "title",
            Author = "author",
            Identifier = "identifier",

            AdditionalInformation = new Dictionary<string, JsonElement>
            {
                ["artistUrl"] = JsonSerializer.SerializeToElement("https://artist.uri/"),
            }.ToImmutableDictionary(),
        };

        var extendedTrack = new ExtendedLavalinkTrack(track);

        // Act
        var artist = extendedTrack.Artist;

        // Assert
        Assert.NotNull(artist);
        Assert.Equal(new Uri("https://artist.uri"), artist.Value.Uri);
    }

    [Fact]
    public void TestArtistPresentIfArtworkUriPresent()
    {
        // Arrange
        var track = new LavalinkTrack
        {
            Title = "title",
            Author = "author",
            Identifier = "identifier",

            AdditionalInformation = new Dictionary<string, JsonElement>
            {
                ["artistArtworkUrl"] = JsonSerializer.SerializeToElement("https://artist.artwork.uri/"),
            }.ToImmutableDictionary(),
        };

        var extendedTrack = new ExtendedLavalinkTrack(track);

        // Act
        var artist = extendedTrack.Artist;

        // Assert
        Assert.NotNull(artist);
        Assert.Equal(new Uri("https://artist.artwork.uri/"), artist.Value.ArtworkUri);
    }

    [Fact]
    public void TestArtistUriPresentWhenBothArtistUriAndArtworkUriPresent()
    {
        // Arrange
        var track = new LavalinkTrack
        {
            Title = "title",
            Author = "author",
            Identifier = "identifier",

            AdditionalInformation = new Dictionary<string, JsonElement>
            {
                ["artistUrl"] = JsonSerializer.SerializeToElement("https://artist.uri/"),
                ["artistArtworkUrl"] = JsonSerializer.SerializeToElement("https://artist.artwork.uri/"),
            }.ToImmutableDictionary(),
        };

        var extendedTrack = new ExtendedLavalinkTrack(track);

        // Act
        var artist = extendedTrack.Artist;

        // Assert
        Assert.NotNull(artist);
        Assert.Equal(new Uri("https://artist.uri"), artist.Value.Uri);
    }

    [Fact]
    public void TestIsPreviewFalseIfNotPresent()
    {
        // Arrange
        var track = new LavalinkTrack
        {
            Title = "title",
            Author = "author",
            Identifier = "identifier",
        };

        var extendedTrack = new ExtendedLavalinkTrack(track);

        // Act
        var isPreview = extendedTrack.IsPreview;

        // Assert
        Assert.False(isPreview);
    }

    [Fact]
    public void TestIsPreviewFalseIfFalse()
    {
        // Arrange
        var track = new LavalinkTrack
        {
            Title = "title",
            Author = "author",
            Identifier = "identifier",

            AdditionalInformation = new Dictionary<string, JsonElement>
            {
                ["isPreview"] = JsonSerializer.SerializeToElement(false),
            }.ToImmutableDictionary(),
        };

        var extendedTrack = new ExtendedLavalinkTrack(track);

        // Act
        var isPreview = extendedTrack.IsPreview;

        // Assert
        Assert.False(isPreview);
    }

    [Fact]
    public void TestIsPreviewTrueIfTrue()
    {
        // Arrange
        var track = new LavalinkTrack
        {
            Title = "title",
            Author = "author",
            Identifier = "identifier",

            AdditionalInformation = new Dictionary<string, JsonElement>
            {
                ["isPreview"] = JsonSerializer.SerializeToElement(true),
            }.ToImmutableDictionary(),
        };

        var extendedTrack = new ExtendedLavalinkTrack(track);

        // Act
        var isPreview = extendedTrack.IsPreview;

        // Assert
        Assert.True(isPreview);
    }

    [Fact]
    public void TestPreviewUriNullIfNotPresent()
    {
        // Arrange
        var track = new LavalinkTrack
        {
            Title = "title",
            Author = "author",
            Identifier = "identifier",
        };

        var extendedTrack = new ExtendedLavalinkTrack(track);

        // Act
        var previewUri = extendedTrack.PreviewUri;

        // Assert
        Assert.Null(previewUri);
    }

    [Fact]
    public void TestPreviewUriNotNullIfPresent()
    {
        // Arrange
        var track = new LavalinkTrack
        {
            Title = "title",
            Author = "author",
            Identifier = "identifier",

            AdditionalInformation = new Dictionary<string, JsonElement>
            {
                ["previewUrl"] = JsonSerializer.SerializeToElement("https://preview.uri/"),
            }.ToImmutableDictionary(),
        };

        var extendedTrack = new ExtendedLavalinkTrack(track);

        // Act
        var previewUri = extendedTrack.PreviewUri;

        // Assert
        Assert.NotNull(previewUri);
        Assert.Equal(new Uri("https://preview.uri/"), previewUri);
    }
}