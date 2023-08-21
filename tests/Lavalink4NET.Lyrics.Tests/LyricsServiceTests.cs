namespace Lavalink4NET.Lyrics.Tests;

using Lavalink4NET.Tracks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

public sealed class LyricsServiceTests
{
    [Fact]
    public async Task TestGetLyricsAsync()
    {
        await using var httpClientFactory = new HttpClientFactory();

        httpClientFactory.Application.MapGet("/v1/{artist}/{title}", async (HttpContext context, string artist, string title) =>
        {
            return TypedResults.Text("""{"lyrics":"Test"}""", "application/json");
        });

        httpClientFactory.Start();

        var track = new LavalinkTrack
        {
            Author = "Rick Astley",
            Identifier = "dQw4w9WgXcQ",
            Title = "Never Gonna Give You Up",
        };

        var lyricsService = new LyricsService(
            httpClientFactory,
            options: Options.Create(new LyricsOptions()),
            memoryCache: null,
            logger: NullLogger<LyricsService>.Instance);

        var lyrics = await lyricsService
            .GetLyricsAsync(track, CancellationToken.None)
            .ConfigureAwait(false);

        Assert.Equal("Test", lyrics);
    }

    [Fact]
    public async Task TestGetLyricsWithCacheHitAsync()
    {
        await using var httpClientFactory = new HttpClientFactory();
        var requestCounter = 0;

        httpClientFactory.Application.MapGet("/v1/{artist}/{title}", async (HttpContext context, string artist, string title) =>
        {
            requestCounter++;
            return TypedResults.Text("""{"lyrics":"Test"}""", "application/json");
        });

        httpClientFactory.Start();

        var track = new LavalinkTrack
        {
            Author = "Rick Astley",
            Identifier = "dQw4w9WgXcQ",
            Title = "Never Gonna Give You Up",
        };

        using var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

        var lyricsService = new LyricsService(
            httpClientFactory,
            options: Options.Create(new LyricsOptions()),
            memoryCache: memoryCache,
            logger: NullLogger<LyricsService>.Instance);

        _ = await lyricsService
            .GetLyricsAsync(track, CancellationToken.None)
            .ConfigureAwait(false);

        _ = await lyricsService
            .GetLyricsAsync(track, CancellationToken.None)
            .ConfigureAwait(false);

        Assert.Equal(1, requestCounter);
    }

    [Fact]
    public async Task TestGetLyricsNullIfRequestFailedAsync()
    {
        await using var httpClientFactory = new HttpClientFactory();

        httpClientFactory.Application.MapGet("/v1/{artist}/{title}", (HttpContext context, string artist, string title) =>
        {
            return TypedResults.StatusCode(500);
        });

        httpClientFactory.Start();

        var track = new LavalinkTrack
        {
            Author = "Rick Astley",
            Identifier = "dQw4w9WgXcQ",
            Title = "Never Gonna Give You Up",
        };

        using var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

        var lyricsService = new LyricsService(
            httpClientFactory,
            options: Options.Create(new LyricsOptions()),
            memoryCache: memoryCache,
            logger: NullLogger<LyricsService>.Instance);

        var result = await lyricsService
            .GetLyricsAsync(track, CancellationToken.None)
            .ConfigureAwait(false);

        Assert.Null(result);
    }

    [Fact]
    public async Task TestGetLyricsThrowsIfRequestFailedIfNotSuppressedAsync()
    {
        await using var httpClientFactory = new HttpClientFactory();

        httpClientFactory.Application.MapGet("/v1/{artist}/{title}", (HttpContext context, string artist, string title) =>
        {
            return TypedResults.StatusCode(500);
        });

        httpClientFactory.Start();

        var track = new LavalinkTrack
        {
            Author = "Rick Astley",
            Identifier = "dQw4w9WgXcQ",
            Title = "Never Gonna Give You Up",
        };

        using var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

        var lyricsService = new LyricsService(
            httpClientFactory,
            options: Options.Create(new LyricsOptions { SuppressExceptions = false, }),
            memoryCache: memoryCache,
            logger: NullLogger<LyricsService>.Instance);

        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            _ = await lyricsService
                .GetLyricsAsync(track, CancellationToken.None)
                .ConfigureAwait(false);
        });
    }
}
