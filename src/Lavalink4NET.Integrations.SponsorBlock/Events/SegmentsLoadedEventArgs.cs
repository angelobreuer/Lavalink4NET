namespace Lavalink4NET.Integrations.SponsorBlock.Events;

using System;
using System.Collections.Immutable;

public sealed class SegmentsLoadedEventArgs : EventArgs
{
    public SegmentsLoadedEventArgs(ulong guildId, ImmutableArray<Segment> segments)
    {
        GuildId = guildId;
        Segments = segments;
    }

    public ulong GuildId { get; }

    public ImmutableArray<Segment> Segments { get; }
}
