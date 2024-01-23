namespace Lavalink4NET.Extensions;

using System;
using Lavalink4NET.DSharpPlus;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// A collection of extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Lavalink4NET DSharpPlus extension to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the extension to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLavalink(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services.AddLavalink<DiscordClientWrapper>();
    }
}
