namespace Lavalink4NET.Integrations.SponsorBlock.Extensions;

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players;

public static class LavalinkPlayerExtensions
{
    public static ValueTask UpdateSponsorBlockCategoriesAsync(
        this ILavalinkPlayer lavalinkPlayer,
        ImmutableArray<SegmentCategory> categories,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(lavalinkPlayer);

        return lavalinkPlayer.ApiClient.UpdateCategoriesAsync(
            sessionId: lavalinkPlayer.SessionId,
            guildId: lavalinkPlayer.GuildId,
            categories: categories,
            cancellationToken: cancellationToken);
    }

    public static ValueTask ResetSponsorBlockCategoriesAsync(
        this ILavalinkPlayer lavalinkPlayer,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(lavalinkPlayer);

        return lavalinkPlayer.ApiClient.ResetCategoriesAsync(
            sessionId: lavalinkPlayer.SessionId,
            guildId: lavalinkPlayer.GuildId,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<ImmutableArray<SegmentCategory>> GetSponsorBlockCategoriesAsync(
        this ILavalinkPlayer lavalinkPlayer,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(lavalinkPlayer);

        return lavalinkPlayer.ApiClient.GetCategoriesAsync(
            sessionId: lavalinkPlayer.SessionId,
            guildId: lavalinkPlayer.GuildId,
            cancellationToken: cancellationToken);
    }
}
