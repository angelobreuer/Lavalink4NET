namespace Lavalink4NET.SponsorBlock;

using System.Collections.Immutable;
using Lavalink4NET.Events;
using Lavalink4NET.SponsorBlock.Event;

public interface ISponsorBlockIntegration
{
    event AsyncEventHandler<SegmentSkippedEventArgs>? SegmentSkipped;

    event AsyncEventHandler<SegmentsLoadedEventArgs>? SegmentsLoaded;

    ImmutableArray<SegmentCategory> DefaultSkipCategories { get; set; }

    ISkipCategories GetCategories(ulong guildId);
}