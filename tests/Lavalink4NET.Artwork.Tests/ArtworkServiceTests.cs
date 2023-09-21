namespace Lavalink4NET.Tests;

using System;
using System.Threading.Tasks;
using Lavalink4NET.Artwork;
using Lavalink4NET.Tracks;

public sealed class ArtworkServiceTests
{
    [Theory]
    [InlineData("https://soundcloud.com/luudeofficial/men-at-work-down-under-luude-remix-1", "https://i1.sndcdn.com/artworks-0WkSQsT8dR9bGMnj-aZU27w-t500x500.jpg")]
    public async Task TestResolveSoundCloudAsync(string uri, string thumbnail)
    {
        // Arrange
        using var artworkService = new ArtworkService();

        var track = new LavalinkTrack
        {
            Identifier = string.Empty,
            Author = string.Empty,
            Duration = default,
            IsLiveStream = false,
            ArtworkUri = new Uri(thumbnail),
            IsSeekable = false,
            Uri = new Uri(uri),
            SourceName = "soundcloud",
            StartPosition = TimeSpan.Zero,
            Title = string.Empty,
        };

        // Act
        var artwork = await artworkService.ResolveAsync(track);

        // Assert
        Assert.NotNull(artwork);
        Assert.Equal(thumbnail, artwork.OriginalString);
    }
}
