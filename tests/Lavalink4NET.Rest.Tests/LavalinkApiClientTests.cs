namespace Lavalink4NET.Rest.Tests;

using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

public class LavalinkApiClientTests
{
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
        Assert.Equal(
            expected: "3.7.3",
            actual: version);
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
}