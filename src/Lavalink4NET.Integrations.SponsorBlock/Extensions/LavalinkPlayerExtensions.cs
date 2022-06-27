namespace Lavalink4NET.Player;

using System;
using Lavalink4NET.Integrations.SponsorBlock;

public static class LavalinkPlayerExtensions
{
    private static ISponsorBlockIntegration GetIntegration(LavalinkPlayer player)
    {
        return player.LavalinkSocket.Integrations.Get<ISponsorBlockIntegration>()
            ?? throw new InvalidOperationException("SponsorBlock is not enabled as an integration for Lavalink4NET.");
    }

    public static ISkipCategories GetCategories(this LavalinkPlayer player)
    {
        return GetIntegration(player).GetCategories(player.GuildId);
    }
}
