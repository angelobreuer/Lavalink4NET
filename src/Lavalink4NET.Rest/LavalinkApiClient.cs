namespace Lavalink4NET.Rest;

using System.Net.Http;
using System.Net.Http.Json;
using Lavalink4NET.Protocol;
using Lavalink4NET.Rest.Entities.Server;
using Lavalink4NET.Rest.Entities.Usage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class LavalinkApiClient : LavalinkApiClientBase
{
    private readonly LavalinkApiClientEndpoints _endpoints;

    public LavalinkApiClient(
        IHttpClientFactory httpClientFactory,
        IOptions<LavalinkApiClientOptions> options,
        ILogger<LavalinkApiClient> logger)
        : base(httpClientFactory, options, logger)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(options);

        _endpoints = new LavalinkApiClientEndpoints(options.Value.BaseAddress);
    }

    public async ValueTask<string> RetrieveVersionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var httpClient = CreateHttpClient();

        return await httpClient
            .GetStringAsync(_endpoints.Version, cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<LavalinkServerInformation> RetrieveServerInformationAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var httpClient = CreateHttpClient();

        var model = await httpClient
            .GetFromJsonAsync(_endpoints.Information, ProtocolSerializerContext.Default.LavalinkServerInformationModel, cancellationToken)
            .ConfigureAwait(false);

        return LavalinkServerInformation.FromModel(model!);
    }

    public async ValueTask<LavalinkServerStatistics> RetrieveStatisticsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var httpClient = CreateHttpClient();

        var model = await httpClient
            .GetFromJsonAsync(_endpoints.Statistics, ProtocolSerializerContext.Default.LavalinkServerStatisticsModel, cancellationToken)
            .ConfigureAwait(false);

        return LavalinkServerStatistics.FromModel(model!);
    }
}
