namespace Lavalink4NET.Tracks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Rest;
using Lavalink4NET.Rest.Entities;
using Lavalink4NET.Rest.Entities.Tracks;

internal sealed class TrackManager : ITrackManager
{
    private readonly ILavalinkApiClient _apiClient;

    public TrackManager(ILavalinkApiClient apiClient)
    {
        ArgumentNullException.ThrowIfNull(apiClient);

        _apiClient = apiClient;
    }

    public ValueTask<LavalinkTrack?> LoadTrackAsync(
        string identifier,
        TrackLoadOptions loadOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(identifier);
        return _apiClient.LoadTrackAsync(identifier, loadOptions, cancellationToken);
    }

    public ValueTask<LavalinkTrack?> LoadTrackAsync(
        string identifier,
        TrackSearchMode searchMode = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(identifier);

        var loadOptions = new TrackLoadOptions(
            SearchMode: searchMode,
            StrictSearch: null,
            CacheMode: CacheMode.Dynamic);

        return LoadTrackAsync(identifier, loadOptions, cancellationToken);
    }

    public ValueTask<TrackLoadResult> LoadTracksAsync(
        string identifier,
        TrackLoadOptions loadOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(identifier);
        return _apiClient.LoadTracksAsync(identifier, loadOptions, cancellationToken);
    }

    public ValueTask<TrackLoadResult> LoadTracksAsync(
        string identifier,
        TrackSearchMode searchMode = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(identifier);

        var loadOptions = new TrackLoadOptions(
            SearchMode: searchMode,
            StrictSearch: null,
            CacheMode: CacheMode.Dynamic);

        return LoadTracksAsync(identifier, loadOptions, cancellationToken);
    }
}
