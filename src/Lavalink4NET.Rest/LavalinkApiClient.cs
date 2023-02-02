namespace Lavalink4NET.Rest;

using System.Collections.Immutable;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Web;
using Lavalink4NET.Protocol;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Protocol.Requests;
using Lavalink4NET.Protocol.Responses;
using Lavalink4NET.Rest.Entities.Server;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Rest.Entities.Usage;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class LavalinkApiClient : LavalinkApiClientBase, ILavalinkApiClient
{
    private readonly LavalinkApiClientEndpoints _endpoints;
    private readonly IMemoryCache _memoryCache;

    public LavalinkApiClient(
        IHttpClientFactory httpClientFactory,
        IOptions<LavalinkApiClientOptions> options,
        IMemoryCache memoryCache,
        ILogger<LavalinkApiClient> logger)
        : base(httpClientFactory, options, logger)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _endpoints = new LavalinkApiClientEndpoints(options.Value.BaseAddress);
        _memoryCache = memoryCache;
    }

    public async ValueTask<LavalinkTrack?> LoadTrackAsync(
        string identifier,
        TrackLoadOptions loadOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(identifier);

        var trackLoadResult = await LoadTracksAsync(
            identifier: identifier,
            loadOptions: loadOptions,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return trackLoadResult.IsSuccess ? trackLoadResult.Track : null;
    }

    private async ValueTask<TrackLoadResponse> LoadTracksInternalAsync(string identifier, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(identifier);

        var queryParameters = HttpUtility.ParseQueryString(string.Empty);
        queryParameters["identifier"] = identifier;

        var requestUri = new UriBuilder(_endpoints.LoadTracks) { Query = queryParameters.ToString(), }.Uri;

        using var httpClient = CreateHttpClient();

        var model = await httpClient
            .GetFromJsonAsync(requestUri, ProtocolSerializerContext.Default.TrackLoadResponse, cancellationToken)
            .ConfigureAwait(false);

        return model!;
    }

    public async ValueTask<TrackLoadResult> LoadTracksAsync(
        string identifier,
        TrackLoadOptions loadOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(identifier);

        identifier = BuildIdentifier(identifier, loadOptions);

        var cacheAccessor = new CacheAccessor<TrackLoadResponse>(
            MemoryCache: _memoryCache,
            Key: $"track-{identifier}",
            Mode: loadOptions.CacheMode);

        if (!cacheAccessor.TryGet(out var response))
        {
            response = await LoadTracksInternalAsync(identifier, cancellationToken).ConfigureAwait(false);

            var success = response.LoadType
                is LoadResultType.TrackLoaded
                or LoadResultType.PlaylistLoaded
                or LoadResultType.SearchResult;

            var relativeExpiration = success
                ? TimeSpan.FromHours(2)
                : TimeSpan.FromMinutes(1);

            cacheAccessor.Set(response, relativeExpiration);
        }

        var tracks = response.Tracks
            .Select(CreateTrack)
            .ToImmutableArray();

        return response.LoadType switch
        {
            LoadResultType.TrackLoaded => TrackLoadResult.CreateTrack(tracks.Single()),

            LoadResultType.PlaylistLoaded => TrackLoadResult.CreatePlaylist(
                tracks: tracks,
                playlist: CreatePlaylist(response, tracks)),

            LoadResultType.SearchResult => TrackLoadResult.CreateSearch(tracks),

            LoadResultType.LoadFailed => TrackLoadResult.CreateLoadFailed(
                exception: new TrackException(
                    Severity: response.Exception!.Severity,
                    Message: response.Exception.Message,
                    Cause: response.Exception.Cause)),

            _ => TrackLoadResult.CreateNoMatches(),
        };

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

    public async ValueTask<string> RetrieveVersionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var httpClient = CreateHttpClient();

        return await httpClient
            .GetStringAsync(_endpoints.Version, cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<PlayerInformationModel> UpdatePlayerAsync(string sessionId, ulong guildId, PlayerUpdateProperties properties, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(properties);

        var requestUri = _endpoints.Player(sessionId, guildId);
        using var httpClient = CreateHttpClient();

        var model = await httpClient
            .GetFromJsonAsync(requestUri, ProtocolSerializerContext.Default.PlayerInformationModel, cancellationToken)
            .ConfigureAwait(false);

        return model!;
    }

    internal static string BuildIdentifier(string identifier, TrackLoadOptions loadOptions = default)
    {
        var strict = loadOptions.StrictSearch.GetValueOrDefault(true);

        if (!strict)
        {
            return loadOptions.SearchMode.Prefix is null
                ? identifier
                : $"{loadOptions.SearchMode.Prefix}:{identifier}";
        }

        var separatorIndex = identifier.AsSpan().IndexOf(':');

        if (separatorIndex is -1)
        {
            return loadOptions.SearchMode.Prefix is null
                ? identifier
                : $"{loadOptions.SearchMode.Prefix}:{identifier}";
        }

        var currentPrefix = identifier[..separatorIndex];

        if (identifier.AsSpan(separatorIndex + 1).StartsWith("//"))
        {
            if (currentPrefix.Equals("https", StringComparison.OrdinalIgnoreCase) ||
                currentPrefix.Equals("http", StringComparison.OrdinalIgnoreCase))
            {
                return identifier;
            }
        }

        throw new InvalidOperationException($"The query '{identifier}' has an search mode specified while search mode is set explicitly and strict mode is enabled.");
    }

    private static PlaylistInformation CreatePlaylist(TrackLoadResponse model, ImmutableArray<LavalinkTrack> tracks)
    {
        var selectedTrack = model.PlaylistInformation!.SelectedTrack is null
            ? null
            : tracks[model.PlaylistInformation.SelectedTrack.Value];

        return new PlaylistInformation(
            Name: model.PlaylistInformation!.Name,
            SelectedTrack: selectedTrack);
    }

    private static LavalinkTrack CreateTrack(TrackModel track) => new()
    {
        Duration = track.Information.Duration,
        Identifier = track.Information.Identifier,
        IsLiveStream = track.Information.IsLiveStream,
        IsSeekable = track.Information.IsSeekable,
        SourceName = track.Information.SourceName,
        StartPosition = track.Information.Position,
        Title = track.Information.Title,
        Uri = track.Information.Uri,
        TrackData = track.Data,
        Author = track.Information.Author,
    };
}
