namespace Lavalink4NET.Integrations.SponsorBlock.Extensions;

using System;
using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Rest;

public static class LavalinkApiClientExtensions
{
    public static async ValueTask<ImmutableArray<SegmentCategory>> GetCategoriesAsync(
        this ILavalinkApiClient apiClient,
        string sessionId,
        ulong guildId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(sessionId);

        var endpoint = apiClient.Endpoints.Build($"/v{apiClient.Endpoints.ApiVersion}/sessions/{sessionId}/players/{guildId}/sponsorblock/categories");
        using var httpClient = apiClient.CreateHttpClient();

        return await httpClient
            .GetFromJsonAsync(endpoint, SponsorBlockJsonSerializerContext.Default.ImmutableArraySegmentCategory, cancellationToken)
            .ConfigureAwait(false);
    }

    public static async ValueTask UpdateCategoriesAsync(
        this ILavalinkApiClient apiClient,
        string sessionId,
        ulong guildId,
        ImmutableArray<SegmentCategory> categories,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(sessionId);

        var endpoint = apiClient.Endpoints.Build($"/v{apiClient.Endpoints.ApiVersion}/sessions/{sessionId}/players/{guildId}/sponsorblock/categories");
        using var httpClient = apiClient.CreateHttpClient();

        using var responseMessage = await httpClient
            .PutAsJsonAsync(endpoint, categories, SponsorBlockJsonSerializerContext.Default.ImmutableArraySegmentCategory, cancellationToken)
            .ConfigureAwait(false);

        responseMessage.EnsureSuccessStatusCode();
    }

    public static async ValueTask ResetCategoriesAsync(
        this ILavalinkApiClient apiClient,
        string sessionId,
        ulong guildId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(sessionId);

        var endpoint = apiClient.Endpoints.Build($"/v{apiClient.Endpoints.ApiVersion}/sessions/{sessionId}/players/{guildId}/sponsorblock/categories");
        using var httpClient = apiClient.CreateHttpClient();

        using var responseMessage = await httpClient
            .DeleteAsync(endpoint, cancellationToken)
            .ConfigureAwait(false);

        responseMessage.EnsureSuccessStatusCode();
    }
}
