namespace Lavalink4NET.Integrations.LyricsJava.Extensions;

using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Text.Json;
using Lavalink4NET.Integrations.LyricsJava.Models;
using Lavalink4NET.Rest;

public static class LavalinkApiClientExtensions
{
    public static async ValueTask<LyricsResponseModel?> GetCurrentTrackLyricsAsync(
        this ILavalinkApiClient apiClient,
        string sessionId,
        ulong guildId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(sessionId);
        ArgumentNullException.ThrowIfNull(guildId);

        var endpoint = apiClient.Endpoints.Build($"/v{apiClient.Endpoints.ApiVersion}/sessions/{sessionId}/players/{guildId}/lyrics");
        using var httpClient = apiClient.CreateHttpClient();

        var response = await httpClient
            .GetAsync(endpoint, cancellationToken)
            .ConfigureAwait(false);

        LyricsResponseModel? result;
        try
        {
            result = await response.Content
                .ReadFromJsonAsync<LyricsResponseModel>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (JsonException)
        {
            result = null;
        }

        return result;
    }

    public static async ValueTask<ImmutableArray<SearchResultModel>> SearchAsync(
        this ILavalinkApiClient apiClient,
        string query,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(query);

        var endpoint = apiClient.Endpoints.Build($"/v{apiClient.Endpoints.ApiVersion}/lyrics/search?query={query}");
        using var httpClient = apiClient.CreateHttpClient();

        var response = await httpClient
            .GetAsync(endpoint, cancellationToken)
            .ConfigureAwait(false);

        ImmutableArray<SearchResultModel> result;
        try
        {
            result = await response.Content
                .ReadFromJsonAsync<ImmutableArray<SearchResultModel>>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (JsonException)
        {
            result = ImmutableArray<SearchResultModel>.Empty;
        }

        return result;
    }

    public static async ValueTask<LyricsResponseModel?> GetYouTubeLyricsAsync(
        this ILavalinkApiClient apiClient,
        string videoId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(videoId);

        var endpoint = apiClient.Endpoints.Build($"/v{apiClient.Endpoints.ApiVersion}/lyrics/{videoId}");
        using var httpClient = apiClient.CreateHttpClient();

        var response = await httpClient
            .GetAsync(endpoint, cancellationToken)
            .ConfigureAwait(false);

        LyricsResponseModel? result;
        try
        {
            result = await response.Content
                .ReadFromJsonAsync<LyricsResponseModel>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (JsonException)
        {
            result = null;
        }

        return result;
    }

    public static async ValueTask<LyricsResponseModel?> GetGeniusLyricsAsync(
        this ILavalinkApiClient apiClient,
        string query,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(query);

        var endpoint = apiClient.Endpoints.Build($"/v{apiClient.Endpoints.ApiVersion}/lyrics/search?query={query}&source=genius");
        using var httpClient = apiClient.CreateHttpClient();

        var response = await httpClient
            .GetAsync(endpoint, cancellationToken)
            .ConfigureAwait(false);

        LyricsResponseModel? result;
        try
        {
            result = await response.Content
                .ReadFromJsonAsync<LyricsResponseModel>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (JsonException)
        {
            result = null;
        }

        return result;
    }
}