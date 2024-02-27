namespace Lavalink4NET.Integrations.SponsorBlock.Players;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players;

public interface ISponsorBlockPlayerListener : ILavalinkPlayerListener
{
    ValueTask NotifySegmentSkippedAsync(Segment segment, CancellationToken cancellationToken = default);

    ValueTask NotifySegmentsLoadedAsync(ImmutableArray<Segment> segments, CancellationToken cancellationToken = default);

    ValueTask NotifyChapterStartedAsync(Chapter chapter, CancellationToken cancellationToken = default);

    ValueTask NotifyChaptersLoadedAsync(ImmutableArray<Chapter> chapters, CancellationToken cancellationToken = default);
}
