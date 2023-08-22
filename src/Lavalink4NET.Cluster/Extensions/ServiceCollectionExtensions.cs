namespace Lavalink4NET.Cluster.Extensions;

using System;
using Lavalink4NET.Clients;
using Lavalink4NET.Cluster;
using Lavalink4NET.Cluster.LoadBalancing;
using Lavalink4NET.Cluster.LoadBalancing.Strategies;
using Lavalink4NET.Cluster.Nodes;
using Lavalink4NET.Integrations;
using Lavalink4NET.Players;
using Lavalink4NET.Rest;
using Lavalink4NET.Socket;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLavalinkClusterCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLogging();
        services.AddHttpClient();
        services.AddMemoryCache();

        services.TryAddSingleton<ISystemClock, SystemClock>();
        services.TryAddSingleton<ILavalinkSocketFactory, LavalinkSocketFactory>();
        services.TryAddSingleton<IClusterAudioService, ClusterAudioService>();
        services.TryAddSingleton<ITrackManager, TrackManager>();
        services.TryAddSingleton<IPlayerManager, PlayerManager>();
        services.TryAddSingleton<IIntegrationManager, IntegrationManager>();
        services.TryAddSingleton<ILavalinkClusterLoadBalancer, LavalinkClusterLoadBalancer>();
        services.TryAddSingleton<ILavalinkApiClientProvider, LavalinkClusterApiClientProvider>();
        services.TryAddSingleton<ILavalinkApiClientFactory, LavalinkApiClientFactory>();
        services.TryAddSingleton<ILavalinkSessionProvider, LavalinkClusterSessionProvider>();

        services.TryAddSingleton<ILavalinkCluster>(x => x.GetRequiredService<IClusterAudioService>());
        services.TryAddSingleton<IAudioService>(x => x.GetRequiredService<IClusterAudioService>());

        services.TryAddSingleton<INodeBalancingStrategy>(
            instance: new RoundRobinBalancingStrategy(
                options: Options.Create(new RoundRobinBalancingStrategyOptions())));

        services.AddHostedService<AudioServiceHost>();

        services.AddOptions<LavalinkClusterNodeOptions>();

        return services;
    }

    public static IServiceCollection AddLavalinkCluster<TClient>(this IServiceCollection services) where TClient : class, IDiscordClientWrapper
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLavalinkClusterCore();

        services.TryAddSingleton<IDiscordClientWrapper, TClient>();

        return services;
    }

    public static IServiceCollection ConfigureLavalinkCluster(this IServiceCollection services, Action<ClusterAudioServiceOptions> options)
    {
        return services.Configure(options);
    }
}
