namespace Lavalink4NET.Extensions;

using System;
using Lavalink4NET.DSharpPlus;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLavalink(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services.AddLavalink<DiscordClientWrapper>();
    }
}
