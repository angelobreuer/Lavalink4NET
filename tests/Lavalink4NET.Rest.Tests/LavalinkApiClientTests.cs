namespace Lavalink4NET.Rest.Tests;

using Lavalink4NET.Protocol.Requests;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

public class LavalinkApiClientTests : IAsyncLifetime
{
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        // Wait maximum 30 seconds for server startup
        using var cancellationTokenSource = new CancellationTokenSource(30000);
        using var httpClient = new HttpClient();

        while (true)
        {
            try
            {
                _ = await httpClient
                    .GetAsync("http://localhost:2333/", cancellationTokenSource.Token)
                    .ConfigureAwait(false);

                break;
            }
            catch (HttpRequestException)
            {
            }
            catch (OperationCanceledException exception)
            {
                throw new TimeoutException("Timed out waiting for lavalink server to start up.", exception);
            }
        }
    }

    [Theory]
    [InlineData("abc", "ytsearch:abc", "ytsearch", true)]
    [InlineData("abc", "scsearch:abc", "scsearch", true)]
    [InlineData("abc", "abc", null, true)]
    [InlineData("https://youtube.com/", "https://youtube.com/", null, true)]
    [InlineData("abc", "abc", null, false)]
    [InlineData("ytsearch:abc", "ytsearch:abc", null, false)]
    [InlineData("scsearch:abc", "scsearch:abc", null, false)]
    [InlineData("othersearch:abc", "othersearch:abc", null, false)]
    public void TestBuildIdentifier(string identifier, string expected, string searchMode, bool strict)
    {
        // Arrange
        var loadOptions = new TrackLoadOptions(
            SearchMode: new TrackSearchMode(searchMode),
            StrictSearch: strict);

        // Act
        var actual = LavalinkApiClient.BuildIdentifier(identifier, loadOptions);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("ytsearch:abc", null)]
    [InlineData("scsearch:abc", null)]
    [InlineData("othersearch:abc", null)]
    [InlineData("ytsearch:abc", "ytsearch")]
    [InlineData("scsearch:abc", "scsearch")]
    public void TestBuildIdentifierFail(string identifier, string? searchMode)
    {
        // Arrange
        var loadOptions = new TrackLoadOptions(
            SearchMode: new TrackSearchMode(searchMode),
            StrictSearch: true);

        // Act
        void Action() => LavalinkApiClient.BuildIdentifier(identifier, loadOptions);

        // Assert
        Assert.Throws<InvalidOperationException>(Action);
    }

    [Fact]
    public async Task TestRetrieveVersion()
    {
        // Arrange
        using var httpClientFactory = new DefaultHttpClientFactory();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: Mock.Of<IMemoryCache>(),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        var version = await client
            .RetrieveVersionAsync()
            .ConfigureAwait(false);

        // Assert
        Assert.NotNull(version);
    }

    [Fact]
    public async Task TestRetrieveInformation()
    {
        // Arrange
        using var httpClientFactory = new DefaultHttpClientFactory();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: Mock.Of<IMemoryCache>(),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        _ = await client
            .RetrieveServerInformationAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task TestRetrieveStatistics()
    {
        // Arrange
        using var httpClientFactory = new DefaultHttpClientFactory();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: Mock.Of<IMemoryCache>(),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        _ = await client
            .RetrieveStatisticsAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task TestLoadTrackAsync()
    {
        // Arrange
        using var httpClientFactory = new DefaultHttpClientFactory();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
                optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        var track = await client
            .LoadTrackAsync("https://www.youtube.com/watch?v=dQw4w9WgXcQ")
            .ConfigureAwait(false);

        // Assert
        Assert.NotNull(track);
        Assert.Contains("Rick Astley", track.Title);
    }

    [Fact]
    public async Task TestSearchTrackAsync()
    {
        // Arrange
        using var httpClientFactory = new DefaultHttpClientFactory();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
                optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        var loadOptions = new TrackLoadOptions(
            SearchMode: TrackSearchMode.YouTube);

        // Act
        var track = await client
            .LoadTrackAsync("Never gonna give you up", loadOptions)
            .ConfigureAwait(false);

        // Assert
        Assert.NotNull(track);
        Assert.Contains("Rick Astley", track.Title);
    }

    [Fact]
    public async Task TestRoutePlannerGetStatusAsync()
    {
        // Arrange
        using var httpClientFactory = new DefaultHttpClientFactory();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
                optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        var routePlannerInformation = await client
            .GetRoutePlannerInformationAsync()
            .ConfigureAwait(false);

        // Assert
        Assert.NotNull(routePlannerInformation);
    }

    [Fact]
    public async Task TestUnmarkAddressAsync()
    {
        // Arrange
        using var httpClientFactory = new DefaultHttpClientFactory();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
                optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        var failedAddress = "/1.0.0.0";

        // Act
        async Task Action() => await client
            .UnmarkFailedAddressAsync(new AddressUnmarkProperties { Address = failedAddress, })
            .ConfigureAwait(false);

        // Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(Action);
        Assert.Contains("Can't access disabled route planner", exception.Message);
    }

    [Fact]
    public async Task TestUnmarkAddressesAsync()
    {
        // Arrange
        using var httpClientFactory = new DefaultHttpClientFactory();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            memoryCache: new MemoryCache(
                optionsAccessor: Options.Create(new MemoryCacheOptions())),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        async Task Action() => await client
            .UnmarkAllFailedAddressesAsync()
            .ConfigureAwait(false);

        // Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(Action);
        Assert.Contains("Can't access disabled route planner", exception.Message);
    }
}