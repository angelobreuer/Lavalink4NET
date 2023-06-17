namespace Lavalink4NET.Integrations.SponsorBlock;

using System;
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
    static SponsorBlockIntegration()
    {
        // Register events
        PayloadJsonConverter.RegisterEvent("SegmentsLoaded", SponsorBlockJsonSerializerContext.Default.SegmentsLoadedEventPayload);
        PayloadJsonConverter.RegisterEvent("SegmentSkipped", SponsorBlockJsonSerializerContext.Default.SegmentSkippedEventPayload);
    }

    public event AsyncEventHandler<SegmentSkippedEventArgs>? SegmentSkipped;

    public event AsyncEventHandler<SegmentsLoadedEventArgs>? SegmentsLoaded;

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
