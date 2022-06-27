namespace Lavalink4NET;

using Lavalink4NET.Integrations.SponsorBlock;

public static class AudioServiceExtensions
{
    public static IAudioService UseSponsorBlock(this IAudioService audioService)
    {
        audioService.Integrations.Set<ISponsorBlockIntegration, SponsorBlockIntegration>(new SponsorBlockIntegration());

        return audioService;
    }
}
