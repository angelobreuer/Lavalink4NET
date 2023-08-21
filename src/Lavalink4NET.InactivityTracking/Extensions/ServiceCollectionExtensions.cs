namespace Lavalink4NET.InactivityTracking.Extensions;

using Lavalink4NET.Extensions;
using Lavalink4NET.InactivityTracking;
using Lavalink4NET.InactivityTracking.Hosting;
using Lavalink4NET.InactivityTracking.Queue;
using Lavalink4NET.Tracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInactivityTracker(this IServiceCollection services, Func<IServiceProvider, IInactivityTracker> implementationFactory)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services.AddSingleton(implementationFactory);
    }

    public static IServiceCollection AddInactivityTracker(this IServiceCollection services, IInactivityTracker inactivityTracker)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton(inactivityTracker);
        return services;
    }

    public static IServiceCollection AddInactivityTracker<T>(this IServiceCollection services) where T : class, IInactivityTracker
    {
        ArgumentNullException.ThrowIfNull(services);
        return services.AddSingleton<IInactivityTracker, T>();
    }

    public static IServiceCollection AddInactivityTracking(this IServiceCollection services)
    {
        services.AddLavalinkCore();

        services.TryAddSingleton<IInactivityTrackingService, InactivityTrackingService>();
        services.TryAddSingleton<IInactivityExpirationQueue, InactivityExpirationQueue>();

        services.AddOptions<InactivityTrackingOptions>();

        services.AddHostedService<InactivityTrackingServiceHost>();
        services.AddHostedService<InactivityExpirationQueueHost>();

        return services;
    }

    public static IServiceCollection ConfigureInactivityTracking(this IServiceCollection services, Action<InactivityTrackingOptions> configure)
    {
        services.Configure(configure);

        return services;
    }

    public static IServiceCollection ConfigureInactivityTracking(this IServiceCollection services, Action<IServiceProvider, InactivityTrackingOptions> configure)
    {
        services.AddSingleton<IConfigureOptions<InactivityTrackingOptions>>(
            serviceProvider => new ConfigureOptions<InactivityTrackingOptions>(serviceProvider, configure));

        return services;
    }
}

file sealed class ConfigureOptions<T> : IConfigureOptions<T> where T : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Action<IServiceProvider, T> _action;

    public ConfigureOptions(IServiceProvider serviceProvider, Action<IServiceProvider, T> action)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(action);

        _serviceProvider = serviceProvider;
        _action = action;
    }

    public void Configure(T options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _action(_serviceProvider, options);
    }
}
