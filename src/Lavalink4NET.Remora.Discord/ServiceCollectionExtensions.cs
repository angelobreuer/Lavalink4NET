namespace Lavalink4NET.Remora.Discord;

using System;
using global::Remora.Discord.Gateway.Extensions;
using Lavalink4NET.Clients;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLavalink(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLavalink<DiscordClientWrapper>();

        services.AddTransient<IDiscordVoiceEventProcessor>(
            static serviceProvider => (DiscordClientWrapper)serviceProvider.GetRequiredService<IDiscordClientWrapper>());

        services.AddResponder<RemoraLavalinkEventResponder>();

        return services;
    }
}
