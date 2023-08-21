namespace Lavalink4NET.Integrations.SponsorBlock;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Events;
using Lavalink4NET.Integrations;
using Lavalink4NET.Integrations.SponsorBlock.Events;
using Lavalink4NET.Integrations.SponsorBlock.Payloads;
using Lavalink4NET.Integrations.SponsorBlock.Players;
using Lavalink4NET.Protocol.Converters;
using Lavalink4NET.Protocol.Payloads;

internal sealed class SponsorBlockIntegration : ILavalinkIntegration, ISponsorBlockIntegration
{
    private readonly IAudioService _audioService;

    static SponsorBlockIntegration()
    {
        // Register events
        PayloadJsonConverter.RegisterEvent("SegmentsLoaded", SponsorBlockJsonSerializerContext.Default.SegmentsLoadedEventPayload);
        PayloadJsonConverter.RegisterEvent("SegmentSkipped", SponsorBlockJsonSerializerContext.Default.SegmentSkippedEventPayload);
    }

    public SponsorBlockIntegration(IAudioService audioService)
    {
        ArgumentNullException.ThrowIfNull(audioService);

        _audioService = audioService;
    }

    public event AsyncEventHandler<SegmentSkippedEventArgs>? SegmentSkipped;

    public event AsyncEventHandler<SegmentsLoadedEventArgs>? SegmentsLoaded;

    async ValueTask ILavalinkIntegration.ProcessPayloadAsync(IPayload payload, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(payload);

        static Segment CreateSegment(SegmentModel model) => new(
            Category: model.Category,
            StartOffset: model.StartOffset,
            EndOffset: model.EndOffset);

        if (payload is SegmentSkippedEventPayload segmentSkippedEventPayload)
        {
            var segment = CreateSegment(segmentSkippedEventPayload.Segment);

            var eventArgs = new SegmentSkippedEventArgs(
                guildId: segmentSkippedEventPayload.GuildId,
                skippedSegment: segment);

            await SegmentSkipped
                .InvokeAsync(this, eventArgs)
                .ConfigureAwait(false);

            var player = await _audioService.Players
                .GetPlayerAsync(segmentSkippedEventPayload.GuildId, cancellationToken)
                .ConfigureAwait(false);

            if (player is ISponsorBlockPlayerListener playerListener)
            {
                await playerListener
                    .NotifySegmentSkippedAsync(segment, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        if (payload is SegmentsLoadedEventPayload segmentsLoadedEventPayload)
        {
            var segments = segmentsLoadedEventPayload.Segments.Select(CreateSegment).ToImmutableArray();

            var eventArgs = new SegmentsLoadedEventArgs(
                guildId: segmentsLoadedEventPayload.GuildId,
                segments: segments);

            await SegmentsLoaded
                .InvokeAsync(this, eventArgs)
                .ConfigureAwait(false);

            var player = await _audioService.Players
                .GetPlayerAsync(segmentsLoadedEventPayload.GuildId, cancellationToken)
                .ConfigureAwait(false);

            if (player is ISponsorBlockPlayerListener playerListener)
            {
                await playerListener
                    .NotifySegmentsLoadedAsync(segments, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
