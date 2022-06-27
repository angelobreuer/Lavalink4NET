namespace Lavalink4NET.SponsorBlock.Event;

using System;
using Lavalink4NET.Player;

public sealed class SegmentSkippedEventArgs : EventArgs
{
    public SegmentSkippedEventArgs(ulong guildId, LavalinkPlayer? player, Segment skippedSegment)
    {
        GuildId = guildId;
        Player = player;
        SkippedSegment = skippedSegment ?? throw new ArgumentNullException(nameof(skippedSegment));
    }

    public ulong GuildId { get; }

    public LavalinkPlayer? Player { get; }

    public Segment SkippedSegment { get; }
}
