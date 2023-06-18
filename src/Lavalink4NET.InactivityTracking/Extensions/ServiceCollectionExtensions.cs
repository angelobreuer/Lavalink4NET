namespace Lavalink4NET.InactivityTracking.Extensions;

using Lavalink4NET.Extensions;
using Lavalink4NET.InactivityTracking;
using Lavalink4NET.InactivityTracking.Hosting;
using Lavalink4NET.Tracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInactivityTracking(this IServiceCollection services)
    {
        services.AddLavalinkCore();

        services.TryAddSingleton<IInactivityTrackingService, InactivityTrackingService>();
        services.AddOptions<InactivityTrackingOptions>();

        services.AddHostedService<InactivityTrackingServiceHost>();

        return services;
    }
}
