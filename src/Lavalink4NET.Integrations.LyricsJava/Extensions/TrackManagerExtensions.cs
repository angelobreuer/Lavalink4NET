namespace Lavalink4NET.Integrations.LyricsJava.Extensions;

using System.Collections.Immutable;
using System.Threading.Tasks;
using Lavalink4NET.Players;
using Lavalink4NET.Tracks;

public static class TrackManagerExtensions
{
    public static async ValueTask<Lyrics?> GetCurrentTrackLyricsAsync(
        this ITrackManager trackManager,
        ILavalinkPlayer player,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(trackManager);
        ArgumentNullException.ThrowIfNull(player);

        var apiClient = await trackManager.ApiClientProvider
            .GetClientAsync(cancellationToken)
            .ConfigureAwait(false);

        return await apiClient
            .GetCurrentTrackLyricsAsync(player.SessionId, player.GuildId, cancellationToken)
            .ConfigureAwait(false);
    }

    public static async ValueTask<ImmutableArray<LyricsSearchResult>> SearchLyricsAsync(
        this ITrackManager trackManager,
        string query,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(trackManager);
        ArgumentNullException.ThrowIfNull(query);

        var apiClient = await trackManager.ApiClientProvider
            .GetClientAsync(cancellationToken)
            .ConfigureAwait(false);

        return await apiClient
            .SearchLyricsAsync(query, cancellationToken)
            .ConfigureAwait(false);
    }

    public static async ValueTask<Lyrics?> GetYouTubeLyricsAsync(
        this ITrackManager trackManager,
        string videoId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(trackManager);
        ArgumentNullException.ThrowIfNull(videoId);

        var apiClient = await trackManager.ApiClientProvider
            .GetClientAsync(cancellationToken)
            .ConfigureAwait(false);

        return await apiClient
            .GetYouTubeLyricsAsync(videoId, cancellationToken)
            .ConfigureAwait(false);
    }

    public static async ValueTask<Lyrics?> GetGeniusLyricsAsync(
        this ITrackManager trackManager,
        string query,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(trackManager);
        ArgumentNullException.ThrowIfNull(query);

        var apiClient = await trackManager.ApiClientProvider
            .GetClientAsync(cancellationToken)
            .ConfigureAwait(false);

        return await apiClient
            .GetGeniusLyricsAsync(query, cancellationToken)
            .ConfigureAwait(false);
    }
}
