namespace Lavalink4NET.Integrations.LyricsJava.Extensions;

public static class AudioServiceExtensions
{
    public static IAudioService UseLyricsJava(this IAudioService audioService)
    {
        ArgumentNullException.ThrowIfNull(audioService);
        
        audioService.Integrations.Set<ILyricsJavaIntegration, LyricsJavaIntegration>(new LyricsJavaIntegration(audioService));

        return audioService;
    }
}