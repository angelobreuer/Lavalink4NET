/*
 *  File:   PlayerStateInfo.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2022
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */

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
