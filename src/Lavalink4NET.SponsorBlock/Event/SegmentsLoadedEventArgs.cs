namespace Lavalink4NET.SponsorBlock.Event;

using System;
using System.Collections.Immutable;
using Lavalink4NET.Player;

public sealed class SegmentsLoadedEventArgs : EventArgs
{
    public SegmentsLoadedEventArgs(ulong guildId, LavalinkPlayer? player, ImmutableArray<Segment> segments)
    {
        GuildId = guildId;
        Player = player;
        Segments = segments;
    }

    public ulong GuildId { get; }
    public LavalinkPlayer? Player { get; }
    public ImmutableArray<Segment> Segments { get; }
}
