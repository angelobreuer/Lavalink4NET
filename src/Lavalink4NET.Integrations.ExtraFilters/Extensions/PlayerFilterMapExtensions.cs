namespace Lavalink4NET.Player;

using System;
using Lavalink4NET.Integrations.ExtraFilters;
using Lavalink4NET.Players;

public static class PlayerFilterMapExtensions
{
    public static void Echo(this IPlayerFilters filters, EchoFilterOptions? filterOptions)
    {
        ArgumentNullException.ThrowIfNull(filters);
        filters.SetFilter(filterOptions);
    }

    public static EchoFilterOptions? Echo(this IPlayerFilters filters)
    {
        ArgumentNullException.ThrowIfNull(filters);
        return filters.GetFilter<EchoFilterOptions>();
    }
}
