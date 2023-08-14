namespace Lavalink4NET.Tracks;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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

        var metricTag = KeyValuePair.Create<string, object?>("Identifier", identifier);

        var apiClient = await resolutionScope
            .GetClientAsync(_apiClientProvider, cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var result = await apiClient
                .LoadTrackAsync(identifier, loadOptions, cancellationToken)
                .ConfigureAwait(false);

            if (result is not null)
            {
                Diagnostics.ResolvedTracks.Add(1, metricTag);
            }
            else
            {
                Diagnostics.FailedQueries.Add(1, metricTag);
            }

            return result;
        }
        catch
        {
            Diagnostics.FailedQueries.Add(1, metricTag);
            throw;
        }
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

        var metricTag = KeyValuePair.Create<string, object?>("Identifier", identifier);

        var apiClient = await resolutionScope
            .GetClientAsync(_apiClientProvider, cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var result = await apiClient
                .LoadTracksAsync(identifier, loadOptions, cancellationToken)
                .ConfigureAwait(false);

            if (result.IsSuccess)
            {
                Diagnostics.ResolvedTracks.Add(1, metricTag);
            }
            else
            {
                Diagnostics.FailedQueries.Add(1, metricTag);
            }

            return result;
        }
        catch
        {
            Diagnostics.FailedQueries.Add(1, metricTag);
            throw;
        }
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

file static class Diagnostics
{
    public static Counter<long> ResolvedTracks { get; }

    public static Counter<long> FailedQueries { get; }

    static Diagnostics()
    {
        var meter = new Meter("Lavalink4NET");

        ResolvedTracks = meter.CreateCounter<long>(
            name: "resolved-tracks",
            unit: "Tracks",
            description: "The number of resolved tracks.");

        FailedQueries = meter.CreateCounter<long>(
            name: "failed-queries",
            unit: "Queries",
            description: "The number of failed track queries.");
    }
}