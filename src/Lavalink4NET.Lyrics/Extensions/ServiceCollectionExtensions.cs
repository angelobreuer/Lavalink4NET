namespace Lavalink4NET.Lyrics.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Internal;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLyrics(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddLogging();

        services.TryAddSingleton<ISystemClock, SystemClock>();
        services.TryAddSingleton<ILyricsService, LyricsService>();

        return services;
    }
}
