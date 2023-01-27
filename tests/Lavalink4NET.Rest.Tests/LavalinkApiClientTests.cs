namespace Lavalink4NET.Rest.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

public class LavalinkApiClientTests
{
    [Fact]
    public async Task TestRetrieveVersion()
    {
        // Arrange
        using var httpClientFactory = new DefaultHttpClientFactory();

        var client = new LavalinkApiClient(
            httpClientFactory: httpClientFactory,
            options: Options.Create(new LavalinkApiClientOptions()),
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        var version = await client
            .RetrieveVersionAsync()
            .ConfigureAwait(false);

        // Assert
        Assert.Equal(
            expected: "d208324989e9c069cc84bbc583d912738949e379-SNAPSHOT",
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
            logger: NullLogger<LavalinkApiClient>.Instance);

        // Act
        _ = await client
            .RetrieveServerInformationAsync()
            .ConfigureAwait(false);
    }
}