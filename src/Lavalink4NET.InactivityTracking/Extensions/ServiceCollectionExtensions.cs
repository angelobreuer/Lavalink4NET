namespace Lavalink4NET.InactivityTracking.Extensions;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Extensions;
using Lavalink4NET.InactivityTracking;
using Lavalink4NET.InactivityTracking.Hosting;
using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;
using Lavalink4NET.Tracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInactivityTracker(this IServiceCollection services, Func<IServiceProvider, IInactivityTracker> implementationFactory, TimeSpan? defaultTimeout = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        return defaultTimeout is not null
            ? services.AddSingleton(serviceProvider => new DefaultTimeoutInactivityTracker(
                inactivityTracker: implementationFactory(serviceProvider),
                defaultTimeoutOverride: defaultTimeout.Value))
            : services.AddSingleton(implementationFactory);
    }

    public static IServiceCollection AddInactivityTracker(this IServiceCollection services, IInactivityTracker inactivityTracker, TimeSpan? defaultTimeout = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        return defaultTimeout is not null
            ? services.AddSingleton(new DefaultTimeoutInactivityTracker(
                inactivityTracker: inactivityTracker,
                defaultTimeoutOverride: defaultTimeout.Value))
            : services.AddSingleton(inactivityTracker);
    }

    public static IServiceCollection AddInactivityTracker<T>(this IServiceCollection services, TimeSpan? defaultTimeout = null) where T : class, IInactivityTracker
    {
        ArgumentNullException.ThrowIfNull(services);

        return defaultTimeout is not null
            ? services.AddSingleton<T>().AddSingleton<IInactivityTracker>(serviceProvider => new DefaultTimeoutInactivityTracker(
                inactivityTracker: serviceProvider.GetRequiredService<T>(),
                defaultTimeoutOverride: defaultTimeout.Value))
            : services.AddSingleton<IInactivityTracker, T>();
    }

    public static IServiceCollection AddInactivityTracking(this IServiceCollection services)
    {
        services.AddLavalinkCore();

        services.TryAddSingleton<IInactivityTrackingService, InactivityTrackingService>();
        services.AddOptions<InactivityTrackingOptions>();

        services.AddHostedService<InactivityTrackingServiceHost>();

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

file sealed class DefaultTimeoutInactivityTracker : IInactivityTracker
{
    private readonly IInactivityTracker _inactivityTracker;
    private readonly TimeSpan _defaultTimeoutOverride;

    public DefaultTimeoutInactivityTracker(IInactivityTracker inactivityTracker, TimeSpan defaultTimeoutOverride)
    {
        ArgumentNullException.ThrowIfNull(inactivityTracker);

        _inactivityTracker = inactivityTracker;
        _defaultTimeoutOverride = defaultTimeoutOverride;
    }

    public async ValueTask<PlayerActivityResult> CheckAsync(InactivityTrackingContext context, ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(player);

        var result = await _inactivityTracker
            .CheckAsync(context, player, cancellationToken)
            .ConfigureAwait(false);

        if (result.Status is PlayerActivityStatus.Inactive)
        {
            return PlayerActivityResult.Inactive(result.Timeout ?? _defaultTimeoutOverride);
        }

        return result;
    }
}