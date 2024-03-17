namespace Lavalink4NET.Integrations.LyricsJava.Extensions;

using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using Lavalink4NET.Integrations.LyricsJava.Models;
using Lavalink4NET.Protocol.Responses;
using Lavalink4NET.Rest;

public static class LavalinkApiClientExtensions
{
    public static async ValueTask<Lyrics?> GetCurrentTrackLyricsAsync(
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

        using var response = await httpClient
            .GetAsync(endpoint, completionOption: HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessStatusCodeAsync(response, cancellationToken).ConfigureAwait(false);

        var result = await response.Content
            .ReadFromJsonWithWorkaroundAsync(ModelJsonSerializerContext.Default.LyricsResponseModel, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return LyricsJavaIntegration.CreateLyrics(result);
    }

    public static async ValueTask<ImmutableArray<LyricsSearchResult>> SearchLyricsAsync(
        this ILavalinkApiClient apiClient,
        string query,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(query);

        var endpoint = apiClient.Endpoints.Build($"/v{apiClient.Endpoints.ApiVersion}/lyrics/search?query={query}");
        using var httpClient = apiClient.CreateHttpClient();

        using var response = await httpClient
            .GetAsync(endpoint, completionOption: HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return ImmutableArray<LyricsSearchResult>.Empty;
        }

        await EnsureSuccessStatusCodeAsync(response, cancellationToken).ConfigureAwait(false);

        var result = await response.Content
            .ReadFromJsonWithWorkaroundAsync(ModelJsonSerializerContext.Default.ImmutableArraySearchResultModel, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return result
            .Select(x => new LyricsSearchResult(x.VideoId, x.Title))
            .ToImmutableArray();
    }

    public static async ValueTask<Lyrics?> GetYouTubeLyricsAsync(
        this ILavalinkApiClient apiClient,
        string videoId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(videoId);

        var endpoint = apiClient.Endpoints.Build($"/v{apiClient.Endpoints.ApiVersion}/lyrics/{videoId}");
        using var httpClient = apiClient.CreateHttpClient();

        using var response = await httpClient
            .GetAsync(endpoint, completionOption: HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessStatusCodeAsync(response, cancellationToken).ConfigureAwait(false);

        var result = await response.Content
            .ReadFromJsonWithWorkaroundAsync(ModelJsonSerializerContext.Default.LyricsResponseModel, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return LyricsJavaIntegration.CreateLyrics(result);
    }

    private static async ValueTask EnsureSuccessStatusCodeAsync(HttpResponseMessage responseMessage, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(responseMessage);

        if (responseMessage.IsSuccessStatusCode)
        {
            return;
        }

        try
        {
            await LavalinkApiClient
                .EnsureSuccessStatusCodeAsync(responseMessage, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (HttpRequestException exception) when (exception.Data.Contains("ErrorResponse") && exception.Data["ErrorResponse"] is HttpErrorResponse errorResponse && errorResponse.ErrorMessage.Contains("\"this.geniusClient\" is null"))
        {
            throw new HttpRequestException("The Genius API key is missing in your server configuration.", exception);
        }
    }

    public static async ValueTask<Lyrics?> GetGeniusLyricsAsync(
        this ILavalinkApiClient apiClient,
        string query,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(query);

        var endpoint = apiClient.Endpoints.Build($"/v{apiClient.Endpoints.ApiVersion}/lyrics/search?query={query}&source=genius");
        using var httpClient = apiClient.CreateHttpClient();

        using var response = await httpClient
            .GetAsync(endpoint, completionOption: HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessStatusCodeAsync(response, cancellationToken).ConfigureAwait(false);

        var result = await response.Content
            .ReadFromJsonWithWorkaroundAsync(ModelJsonSerializerContext.Default.LyricsResponseModel, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return LyricsJavaIntegration.CreateLyrics(result);
    }
}

file static class LyricsJavaWorkaround
{
    public static async Task<T?> ReadFromJsonWithWorkaroundAsync<T>(this HttpContent content, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken = default)
    {
        using var jsonObject = await content
            .ReadFromJsonAsync<JsonDocument>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var fixedJsonObject = new JsonObject { ["type"] = JsonValue.Create(jsonObject!.RootElement.GetProperty("type")), };

        foreach (var element in jsonObject.RootElement.EnumerateObject().Where(x => !x.Name.Equals("type", StringComparison.Ordinal)))
        {
            fixedJsonObject.Add(element.Name, element.Value.ValueKind switch
            {
                JsonValueKind.Object => JsonObject.Create(element.Value),
                JsonValueKind.Array => JsonArray.Create(element.Value),
                JsonValueKind.True or JsonValueKind.False or JsonValueKind.String or JsonValueKind.Number => JsonValue.Create(element.Value),
                _ => JsonValue.Create((string?)null),
            });
        }

        return fixedJsonObject.Deserialize(jsonTypeInfo);
    }
}