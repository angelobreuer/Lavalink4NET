namespace Lavalink4NET.Integrations.LyricsJava.Extensions;

using Microsoft.Extensions.Options;

public static class AudioServiceExtensions
{
    public static IAudioService UseLyricsJava(this IAudioService audioService, IOptions<LyricsJavaIntegrationOptions> options)
    {
        ArgumentNullException.ThrowIfNull(audioService);
        ArgumentNullException.ThrowIfNull(options);

        audioService.Integrations.Set<ILyricsJavaIntegration, LyricsJavaIntegration>(new LyricsJavaIntegration(audioService, options));

        return audioService;
    }

    public static IAudioService UseLyricsJava(this IAudioService audioService, Action<LyricsJavaIntegrationOptions>? configure)
    {
        ArgumentNullException.ThrowIfNull(audioService);

        var options = new LyricsJavaIntegrationOptions();
        configure?.Invoke(options);

        return audioService.UseLyricsJava(Options.Create(options));
    }

    public static IAudioService UseLyricsJava(this IAudioService audioService, LyricsJavaIntegrationOptions options)
    {
        ArgumentNullException.ThrowIfNull(audioService);
        ArgumentNullException.ThrowIfNull(options);

        return audioService.UseLyricsJava(Options.Create(options));
    }

    public static IAudioService UseLyricsJava(this IAudioService audioService)
    {
        ArgumentNullException.ThrowIfNull(audioService);

        return audioService.UseLyricsJava(Options.Create(new LyricsJavaIntegrationOptions()));
    }
}