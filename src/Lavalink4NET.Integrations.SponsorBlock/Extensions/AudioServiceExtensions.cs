namespace Lavalink4NET;

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Integrations.SponsorBlock;
using Lavalink4NET.Integrations.SponsorBlock.Extensions;
using Lavalink4NET.Players;

public static class AudioServiceExtensions
{
    public static IAudioService UseSponsorBlock(this IAudioService audioService)
    {
        ArgumentNullException.ThrowIfNull(audioService);

        audioService.Integrations.Set<ISponsorBlockIntegration, SponsorBlockIntegration>(new SponsorBlockIntegration());

        return audioService;
    }

    public static ValueTask UpdateSponsorBlockCategoriesAsync(
        this IAudioService audioService,
        ILavalinkPlayer lavalinkPlayer,
        ImmutableArray<SegmentCategory> categories,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(audioService);
        ArgumentNullException.ThrowIfNull(lavalinkPlayer);

        return lavalinkPlayer.ApiClient.UpdateCategoriesAsync(
            sessionId: lavalinkPlayer.SessionId,
            guildId: lavalinkPlayer.GuildId,
            categories: categories,
            cancellationToken: cancellationToken);
    }

    public static ValueTask ResetSponsorBlockCategoriesAsync(
        this IAudioService audioService,
        ILavalinkPlayer lavalinkPlayer,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(audioService);

        return lavalinkPlayer.ApiClient.ResetCategoriesAsync(
            sessionId: lavalinkPlayer.SessionId,
            guildId: lavalinkPlayer.GuildId,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<ImmutableArray<SegmentCategory>> GetSponsorBlockCategoriesAsync(
        this IAudioService audioService,
        ILavalinkPlayer lavalinkPlayer,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(audioService);

        return lavalinkPlayer.ApiClient.GetCategoriesAsync(
            sessionId: lavalinkPlayer.SessionId,
            guildId: lavalinkPlayer.GuildId,
            cancellationToken: cancellationToken);
    }
}
