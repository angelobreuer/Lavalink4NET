namespace Lavalink4NET.Integrations.Lavasearch.Extensions;

using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using Lavalink4NET.Integrations.Lavasearch.Models;
using Lavalink4NET.Rest;
using Lavalink4NET.Rest.Entities.Tracks;

public static class LavalinkApiClientExtensions
{
    public static async ValueTask<SearchResultModel?> SearchAsync(
        this ILavalinkApiClient apiClient,
        string query,
        ImmutableArray<SearchCategory>? categories = null,
        TrackLoadOptions loadOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(apiClient);

        static string MapCategory(SearchCategory category) => category switch
        {
            SearchCategory.Track => "track",
            SearchCategory.Album => "album",
            SearchCategory.Artist => "artist",
            SearchCategory.Playlist => "playlist",
            SearchCategory.Text => "text",
            _ => throw new InvalidOperationException(),
        };

        var endpoint = apiClient.Endpoints.Build($"/v{apiClient.Endpoints.ApiVersion}/loadsearch");

        var queryParameters = HttpUtility.ParseQueryString(string.Empty);

        queryParameters["types"] = categories is not null
            ? string.Join(',', categories.Value.Select(MapCategory))
            : "track";

        queryParameters["query"] = LavalinkApiClient.BuildIdentifier(query, loadOptions);

        var requestUri = new UriBuilder(endpoint) { Query = queryParameters.ToString(), }.Uri;

        using var httpClient = apiClient.CreateHttpClient();

        using var responseMessage = await httpClient
            .GetAsync(requestUri, cancellationToken)
            .ConfigureAwait(false);

        if (responseMessage.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }

        await LavalinkApiClient
            .EnsureSuccessStatusCodeAsync(responseMessage, cancellationToken)
            .ConfigureAwait(false);

        var model = await responseMessage.Content
            .ReadFromJsonAsync(ModelJsonSerializerContext.Default.SearchResultModel, cancellationToken)
            .ConfigureAwait(false);

        return model!;
    }
}
