namespace Lavalink4NET.Integrations.LyricsJava.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class HostExtensions
{
    public static IHost UseLyricsJava(this IHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        host.Services.GetRequiredService<IAudioService>().UseLyricsJava();
        return host;
    }
}