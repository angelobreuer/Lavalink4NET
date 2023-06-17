namespace Lavalink4NET.Extensions;

using System;
using Lavalink4NET.Clients;
using Lavalink4NET.Integrations;
using Lavalink4NET.Players;
using Lavalink4NET.Rest;
using Lavalink4NET.Socket;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Internal;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLavalinkCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLogging();
        services.AddHttpClient();
        services.AddMemoryCache();

        services.TryAddSingleton<ISystemClock, SystemClock>();
        services.TryAddSingleton<ILavalinkSocketFactory, LavalinkSocketFactory>();
        services.TryAddSingleton<ILavalinkApiClient, LavalinkApiClient>();
        services.TryAddSingleton<ILavalinkSessionProvider, LavalinkSessionProvider>();
        services.TryAddSingleton<IAudioService, AudioService>();
        services.TryAddSingleton<ITrackManager, TrackManager>();
        services.TryAddSingleton<IPlayerManager, PlayerManager>();
        services.TryAddSingleton<IIntegrationManager, IntegrationManager>();

        services.AddHostedService<AudioServiceHost>();

        services.AddOptions<LavalinkNodeOptions>();

        return services;
    }

    public static IServiceCollection AddLavalink<TClient>(this IServiceCollection services) where TClient : class, IDiscordClientWrapper
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLavalinkCore();

        services.TryAddSingleton<IDiscordClientWrapper, TClient>();

        return services;
    }

    public static IServiceCollection ConfigureLavalink(this IServiceCollection services, Action<LavalinkNodeOptions> options)
    {
        return services.Configure(options);
    }
}
