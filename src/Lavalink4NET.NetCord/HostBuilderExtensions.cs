namespace Lavalink4NET.NetCord;

using System;
using Microsoft.Extensions.Hosting;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseLavalink(this IHostBuilder hostBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

        return hostBuilder.ConfigureServices(static (_, services) => services.AddLavalink());
    }
}
