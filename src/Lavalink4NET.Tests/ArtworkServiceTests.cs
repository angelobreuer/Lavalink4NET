namespace Lavalink4NET.Tests;

using System.Threading.Tasks;
using Lavalink4NET.Artwork;
using Lavalink4NET.Player;
using Xunit;

public sealed class ArtworkServiceTests
{
    [Theory]
    [InlineData("https://soundcloud.com/luudeofficial/men-at-work-down-under-luude-remix-1", "https://i1.sndcdn.com/artworks-0WkSQsT8dR9bGMnj-aZU27w-t500x500.jpg")]
    public async Task TestResolveSoundCloudAsync(string uri, string thumbnail)
    {
        using var artworkService = new ArtworkService();
        var track = new LavalinkTrack(string.Empty, string.Empty, default, false, false, uri, string.Empty, uri, StreamProvider.SoundCloud);
        var artwork = await artworkService.ResolveAsync(track);
        Assert.Equal(thumbnail, artwork.OriginalString);
    }
}
