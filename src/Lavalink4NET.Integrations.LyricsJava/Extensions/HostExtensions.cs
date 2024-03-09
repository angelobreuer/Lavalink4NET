namespace Lavalink4NET.Integrations.LyricsJava.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

public static class HostExtensions
{
    public static IAudioService UseLyricsJava(this IHost host, IOptions<LyricsJavaIntegrationOptions> options)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(options);

        return host.Services.GetRequiredService<IAudioService>().UseLyricsJava(options);
    }

    public static IAudioService UseLyricsJava(this IHost host, Action<LyricsJavaIntegrationOptions>? configure)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(configure);

        return host.Services.GetRequiredService<IAudioService>().UseLyricsJava(configure);
    }

    public static IAudioService UseLyricsJava(this IHost host, LyricsJavaIntegrationOptions options)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(options);

        return host.Services.GetRequiredService<IAudioService>().UseLyricsJava(options);
    }

    public static IAudioService UseLyricsJava(this IHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        return host.Services.GetRequiredService<IAudioService>().UseLyricsJava();
    }
}