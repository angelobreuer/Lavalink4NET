namespace Lavalink4NET;

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Integrations.SponsorBlock;
using Lavalink4NET.Integrations.SponsorBlock.Extensions;

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
        ulong guildId,
        ImmutableArray<SegmentCategory> categories,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(audioService);
        var sessionId = GetSessionInternal(audioService);

        return audioService.ApiClient.UpdateCategoriesAsync(sessionId, guildId, categories, cancellationToken);
    }

    public static ValueTask ResetSponsorBlockCategoriesAsync(
        this IAudioService audioService,
        ulong guildId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(audioService);
        var sessionId = GetSessionInternal(audioService);

        return audioService.ApiClient.ResetCategoriesAsync(sessionId, guildId, cancellationToken);
    }

    public static ValueTask<ImmutableArray<SegmentCategory>> GetSponsorBlockCategoriesAsync(
        this IAudioService audioService,
        ulong guildId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(audioService);
        var sessionId = GetSessionInternal(audioService);

        return audioService.ApiClient.GetCategoriesAsync(sessionId, guildId, cancellationToken);
    }

    private static string GetSessionInternal(IAudioService audioService)
    {
        ArgumentNullException.ThrowIfNull(audioService);
        return audioService.SessionId ?? throw new InvalidOperationException("The audio service is not ready.");
    }
}
