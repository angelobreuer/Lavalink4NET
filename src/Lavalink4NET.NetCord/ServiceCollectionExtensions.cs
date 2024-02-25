namespace Lavalink4NET.NetCord;

using System;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLavalink(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLavalink<DiscordClientWrapper>();

        return services;
    }
}
