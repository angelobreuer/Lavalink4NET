namespace Lavalink4NET.Integrations.SponsorBlock;

using System.Collections.Immutable;
using Lavalink4NET.Events;
using Lavalink4NET.Integrations.SponsorBlock.Events;

public interface ISponsorBlockIntegration
{
    event AsyncEventHandler<SegmentSkippedEventArgs>? SegmentSkipped;

    event AsyncEventHandler<SegmentsLoadedEventArgs>? SegmentsLoaded;

    ImmutableArray<SegmentCategory> DefaultSkipCategories { get; set; }

    ISkipCategories GetCategories(ulong guildId);
}