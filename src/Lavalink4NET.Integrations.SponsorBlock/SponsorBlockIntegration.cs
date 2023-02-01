namespace Lavalink4NET.Integrations.SponsorBlock;

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Events;
using Lavalink4NET.Integrations;
using Lavalink4NET.Integrations.SponsorBlock.Events;
using Lavalink4NET.Integrations.SponsorBlock.Payloads;
using Lavalink4NET.Protocol.Converters;
using Lavalink4NET.Protocol.Payloads;

internal sealed class SponsorBlockIntegration : ILavalinkIntegration, ISponsorBlockIntegration
{
    private readonly ConcurrentDictionary<ulong, ISkipCategories> _skipCategories;

    public SponsorBlockIntegration()
    {
        _skipCategories = new ConcurrentDictionary<ulong, ISkipCategories>();

        // Register events
        PayloadJsonConverter.RegisterEvent<SegmentsLoadedEventPayload>("SegmentsLoaded");
        PayloadJsonConverter.RegisterEvent<SegmentSkippedEventPayload>("SegmentSkipped");

        throw new NotSupportedException("The SponsorBlock integration is currently not available for the newest Lavalink4NET. The support is blocked by https://github.com/TopiSenpai/Sponsorblock-Plugin/pull/6.");
    }

    public event AsyncEventHandler<SegmentSkippedEventArgs>? SegmentSkipped;

    public event AsyncEventHandler<SegmentsLoadedEventArgs>? SegmentsLoaded;

    public ImmutableArray<SegmentCategory> DefaultSkipCategories { get; set; }

    public ISkipCategories GetCategories(ulong guildId)
    {
        return _skipCategories.GetOrAdd(guildId, _ => new SkipCategoriesCollection(this));
    }

    async ValueTask ILavalinkIntegration.ProcessPayloadAsync(IPayload payload, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(payload);

        if (payload is SegmentSkippedEventPayload segmentSkippedEventPayload)
        {
            var eventArgs = new SegmentSkippedEventArgs(
                guildId: segmentSkippedEventPayload.GuildId,
                skippedSegment: segmentSkippedEventPayload.Segment);

            await SegmentSkipped
                .InvokeAsync(this, eventArgs)
                .ConfigureAwait(false);
        }

        if (payload is SegmentsLoadedEventPayload segmentsLoadedEventPayload)
        {
            var eventArgs = new SegmentsLoadedEventArgs(
                guildId: segmentsLoadedEventPayload.GuildId,
                segments: segmentsLoadedEventPayload.Segments);

            await SegmentsLoaded
                .InvokeAsync(this, eventArgs)
                .ConfigureAwait(false);
        }
    }
}
