namespace Lavalink4NET.Integrations.SponsorBlock.Extensions;

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class HostExtensions
{
    public static IHost UseSponsorBlock(this IHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        host.Services.GetRequiredService<IAudioService>().UseSponsorBlock();
        return host;
    }
}
