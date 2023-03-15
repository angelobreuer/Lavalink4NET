namespace Lavalink4NET.Rest;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Protocol.Requests;
using Lavalink4NET.Rest.Entities.Server;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Rest.Entities.Usage;
using Lavalink4NET.Tracks;

public interface ILavalinkApiClient
{
    LavalinkApiEndpoints Endpoints { get; }

    HttpClient CreateHttpClient();

    ValueTask<LavalinkTrack?> LoadTrackAsync(
        string identifier,
        TrackLoadOptions loadOptions = default,
        CancellationToken cancellationToken = default);

    ValueTask<TrackLoadResult> LoadTracksAsync(
        string identifier,
        TrackLoadOptions loadOptions = default,
        CancellationToken cancellationToken = default);

    ValueTask<LavalinkServerInformation> RetrieveServerInformationAsync(CancellationToken cancellationToken = default);

    ValueTask<LavalinkServerStatistics> RetrieveStatisticsAsync(CancellationToken cancellationToken = default);

    ValueTask<string> RetrieveVersionAsync(CancellationToken cancellationToken = default);

    ValueTask<PlayerInformationModel> UpdatePlayerAsync(string sessionId, ulong guildId, PlayerUpdateProperties properties, CancellationToken cancellationToken = default);
}