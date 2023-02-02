namespace Lavalink4NET.Extensions;

using System;
using Lavalink4NET.Clients;
using Lavalink4NET.Players;
using Lavalink4NET.Rest;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLavalink(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLogging();
        services.AddHttpClient();
        services.AddMemoryCache();

        services.TryAddSingleton<ILavalinkApiClient, LavalinkApiClient>();
        services.TryAddSingleton<IAudioService, AudioService>();
        services.TryAddSingleton<ITrackManager, TrackManager>();

        return services;
    }

    public static IServiceCollection AddLavalink<TClient>(this IServiceCollection services) where TClient : class, IDiscordClientWrapper
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLavalink();

        services.TryAddSingleton<IDiscordClientWrapper, TClient>();

        return services;
    }
}
