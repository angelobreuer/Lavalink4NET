namespace Lavalink4NET.Integrations.LyricsJava.Extensions;

using System.Collections.Immutable;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using Lavalink4NET.Integrations.LyricsJava.Models;
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

        var response = await httpClient
            .GetAsync(endpoint, completionOption: HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }

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

        var response = await httpClient
            .GetAsync(endpoint, completionOption: HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return ImmutableArray<LyricsSearchResult>.Empty;
        }

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

        var response = await httpClient
            .GetAsync(endpoint, completionOption: HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }

        var result = await response.Content
            .ReadFromJsonWithWorkaroundAsync(ModelJsonSerializerContext.Default.LyricsResponseModel, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return LyricsJavaIntegration.CreateLyrics(result);
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

        var response = await httpClient
            .GetAsync(endpoint, completionOption: HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }

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
        // FIXME: LyricsJava somehow returns the type property twice in the object
        cancellationToken.ThrowIfCancellationRequested();

        T? Parse(ReadOnlySpan<byte> value)
        {
            var jsonObject = new JsonObject();
            var utf8JsonReader = new Utf8JsonReader(value);
            var isFirst = true;

            if (!utf8JsonReader.Read())
            {
                throw new JsonException("Unexpected EOF", new EndOfStreamException());
            }

            while (utf8JsonReader.Read() && utf8JsonReader.TokenType is not JsonTokenType.EndObject)
            {
                var propertyName = utf8JsonReader.GetString()!;

                if (!utf8JsonReader.Read())
                {
                    throw new JsonException("Unexpected EOF", new EndOfStreamException());
                }

                if (!isFirst && propertyName.Equals("type", StringComparison.Ordinal))
                {
                    continue; // Fix duplicate property
                }

                jsonObject[propertyName] = JsonNode.Parse(ref utf8JsonReader);
                isFirst = true;
            }

            return jsonObject.Deserialize(jsonTypeInfo);
        }

        var jsonObject = await content
            .ReadAsByteArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        return Parse(jsonObject);
    }
}