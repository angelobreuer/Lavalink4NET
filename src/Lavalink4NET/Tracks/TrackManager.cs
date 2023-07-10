namespace Lavalink4NET.Tracks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Rest;
using Lavalink4NET.Rest.Entities;
using Lavalink4NET.Rest.Entities.Tracks;

internal sealed class TrackManager : ITrackManager
{
    private readonly ILavalinkApiClientProvider _apiClientProvider;

    public TrackManager(ILavalinkApiClientProvider apiClientProvider)
    {
        ArgumentNullException.ThrowIfNull(apiClientProvider);

        _apiClientProvider = apiClientProvider;
    }

    public async ValueTask<LavalinkTrack?> LoadTrackAsync(
        string identifier,
        TrackLoadOptions loadOptions = default,
        LavalinkApiResolutionScope resolutionScope = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(identifier);

        var apiClient = await resolutionScope
            .GetClientAsync(_apiClientProvider, cancellationToken)
            .ConfigureAwait(false);

        return await apiClient
            .LoadTrackAsync(identifier, loadOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    public ValueTask<LavalinkTrack?> LoadTrackAsync(
        string identifier,
        TrackSearchMode searchMode = default,
        LavalinkApiResolutionScope resolutionScope = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(identifier);

        var loadOptions = new TrackLoadOptions(
            SearchMode: searchMode,
            StrictSearch: null,
            CacheMode: CacheMode.Dynamic);

        return LoadTrackAsync(identifier, loadOptions, resolutionScope, cancellationToken);
    }

    public async ValueTask<TrackLoadResult> LoadTracksAsync(
        string identifier,
        TrackLoadOptions loadOptions = default,
        LavalinkApiResolutionScope resolutionScope = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(identifier);

        var apiClient = await resolutionScope
            .GetClientAsync(_apiClientProvider, cancellationToken)
            .ConfigureAwait(false);

        return await apiClient
            .LoadTracksAsync(identifier, loadOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    public ValueTask<TrackLoadResult> LoadTracksAsync(
        string identifier,
        TrackSearchMode searchMode = default,
        LavalinkApiResolutionScope resolutionScope = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(identifier);

        var loadOptions = new TrackLoadOptions(
            SearchMode: searchMode,
            StrictSearch: null,
            CacheMode: CacheMode.Dynamic);

        return LoadTracksAsync(identifier, loadOptions, resolutionScope, cancellationToken);
    }
}
