namespace Lavalink4NET.Integrations.SponsorBlock.Events;

using System;

public sealed class SegmentSkippedEventArgs : EventArgs
{
    public SegmentSkippedEventArgs(ulong guildId, Segment skippedSegment)
    {
        ArgumentNullException.ThrowIfNull(skippedSegment);

        GuildId = guildId;
        SkippedSegment = skippedSegment;
    }

    public ulong GuildId { get; }

    public Segment SkippedSegment { get; }
}
