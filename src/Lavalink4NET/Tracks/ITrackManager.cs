namespace Lavalink4NET.Tracks;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Rest;
using Lavalink4NET.Rest.Entities.Tracks;

public interface ITrackManager
{
    ValueTask<LavalinkTrack?> LoadTrackAsync(
        string identifier,
        TrackLoadOptions loadOptions = default,
        LavalinkApiResolutionScope resolutionScope = default,
        CancellationToken cancellationToken = default);

    ValueTask<TrackLoadResult> LoadTracksAsync(
        string identifier,
        TrackLoadOptions loadOptions = default,
        LavalinkApiResolutionScope resolutionScope = default,
        CancellationToken cancellationToken = default);

    ValueTask<LavalinkTrack?> LoadTrackAsync(
        string identifier,
        TrackSearchMode searchMode = default,
        LavalinkApiResolutionScope resolutionScope = default,
        CancellationToken cancellationToken = default);

    ValueTask<TrackLoadResult> LoadTracksAsync(
        string identifier,
        TrackSearchMode searchMode = default,
        LavalinkApiResolutionScope resolutionScope = default,
        CancellationToken cancellationToken = default);
}
