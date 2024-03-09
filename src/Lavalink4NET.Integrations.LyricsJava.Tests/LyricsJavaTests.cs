namespace Lavalink4NET.Integrations.LyricsJava.Tests;

using System.Threading.Tasks;
using Lavalink4NET.Integrations.LyricsJava.Extensions;
using Lavalink4NET.Rest;
using Lavalink4NET.Rest.Tests;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public sealed class LyricsJavaTests
{
    [Fact]
    public async Task TestGetGeniusLyricsAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();

        httpClientFactory.Application.MapGet("/v4/lyrics/search", (HttpContext context) =>
        {
            Assert.Equal("Never Gonna Give You Up", context.Request.Query["query"].ToString());
            Assert.Equal("genius", context.Request.Query["source"].ToString());

            return TypedResults.Text("""
                {
                	"type": "text",
                	"track": {
                		"title": "Never Gonna Give You Up",
                		"author": "Rick Astley",
                		"album": null,
                		"albumArt": [
                			{
                				"url": "https://images.genius.com/88634fdafc60d4ff1e76944436c34a19.901x901x1.png",
                				"height": -1,
                				"width": -1
                			}
                		]
                	},
                	"source": "genius.com",
                	"text": "[Intro]\nDesert you\nOoh-ooh-ooh-ooh\nHurt you\n\n[Verse 1]\nWe're no strangers to love\nYou know the rules and so do I (Do I)\nA full commitment's what I'm thinking of\nYou wouldn't get this from any other guy\n\n[Pre-Chorus]\nI just wanna tell you how I'm feeling\nGotta make you understand\n\n[Chorus]\nNever gonna give you up\nNever gonna let you down\nNever gonna run around and desert you\nNever gonna make you cry\nNever gonna say goodbye\nNever gonna tell a lie and hurt you\n\n[Verse 2]\nWe've known each other for so long\nYour heart's been aching, but you're too shy to say it (To say it)\nInside, we both know what's been going on (Going on)\nWe know the game, and we're gonna play it",
                	"type": "text"
                }
                """,
                contentType: "application/json");
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: Mock.Of<IMemoryCache>(),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        var lyrics = await client
            .GetGeniusLyricsAsync("Never Gonna Give You Up")
            .ConfigureAwait(false);

        // Assert
        Assert.NotNull(lyrics);
        Assert.Contains("Desert you", lyrics.Text);
    }

    [Fact]
    public async Task TestGetYouTubeLyricsNotFoundAsync()
    {
        // Arrange
        await using var httpClientFactory = new HttpClientFactory();

        httpClientFactory.Application.MapGet("/v4/lyrics/search", (HttpContext context) =>
        {
            Assert.Equal("Never Gonna Give You Up", context.Request.Query["query"].ToString());
            Assert.Empty(context.Request.Query["source"].ToString());

            return TypedResults.Text("""
                {
                	"timestamp": 1709994651386,
                	"status": 404,
                	"error": "Not Found",
                	"message": "Lyrics were not found",
                	"path": "/v4/lyrics/fJ9rUzIMcZQ"
                }
                """,
                contentType: "application/json",
                statusCode: 404);
        });

        httpClientFactory.Start();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: Mock.Of<IMemoryCache>(),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        var lyrics = await client
            .GetYouTubeLyricsAsync("dQw4w9WgXcQ")
            .ConfigureAwait(false);

        // Assert
        Assert.Null(lyrics);
    }
}
