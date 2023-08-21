namespace Lavalink4NET.Integrations.SponsorBlock;

using Lavalink4NET.Events;
using Lavalink4NET.Integrations.SponsorBlock.Events;

public interface ISponsorBlockIntegration
{
    event AsyncEventHandler<SegmentSkippedEventArgs>? SegmentSkipped;

    event AsyncEventHandler<SegmentsLoadedEventArgs>? SegmentsLoaded;
}