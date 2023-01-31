namespace Lavalink4NET.Player;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lavalink4NET.Filters;

public sealed class PlayerStateInfo
{
    private static readonly Dictionary<string, IFilterOptions> _noFiltersSentinel = new();

    private PlayerStateInfo(ulong guildId, TimeSpan trackPosition, LavalinkTrack? currentTrack, IReadOnlyDictionary<string, IFilterOptions> filters)
    {
        GuildId = guildId;
        TrackPosition = trackPosition;
        CurrentTrack = currentTrack;
        Filters = filters ?? throw new ArgumentNullException(nameof(filters));
    }

    public static PlayerStateInfo Capture(LavalinkPlayer player)
    {
        if (player is null)
        {
            throw new ArgumentNullException(nameof(player));
        }

        var filters = player.HasFilters
            ? new Dictionary<string, IFilterOptions>(player.Filters.Filters)
            : _noFiltersSentinel;

        return new PlayerStateInfo(
            guildId: player.GuildId,
            trackPosition: player.TrackPosition,
            currentTrack: player.CurrentTrack,
            filters: filters);
    }

    public ulong GuildId { get; }

    public TimeSpan TrackPosition { get; }

    public LavalinkTrack? CurrentTrack { get; }

    public IReadOnlyDictionary<string, IFilterOptions> Filters { get; }

    public async Task RestoreAsync(LavalinkPlayer player)
    {
        // play track if one is playing
        if (CurrentTrack != null)
        {
            // restart track
            await player.PlayAsync(CurrentTrack, TrackPosition);
        }

        // apply filters
        if (!ReferenceEquals(Filters, _noFiltersSentinel))
        {
            player.Filters.Filters = (Dictionary<string, IFilterOptions>)Filters;
            await player.Filters.CommitAsync(force: true);
        }
    }
}
