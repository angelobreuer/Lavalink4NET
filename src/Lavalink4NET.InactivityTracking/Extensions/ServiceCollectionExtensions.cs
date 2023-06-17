namespace Lavalink4NET.InactivityTracking.Extensions;

using Lavalink4NET.Extensions;
using Lavalink4NET.Tracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInactivityTracking(this IServiceCollection services)
    {
        services.AddLavalinkCore();

        services.TryAddSingleton<IInactivityTrackingService, InactivityTrackingService>();

        return services;
    }
}
