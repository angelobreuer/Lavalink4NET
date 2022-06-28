namespace Lavalink4NET.Integrations.SponsorBlock;

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Events;
using Lavalink4NET.Integrations;
using Lavalink4NET.Integrations.SponsorBlock.Event;
using Lavalink4NET.Integrations.SponsorBlock.Payloads;
using Lavalink4NET.Payloads;

internal sealed class SponsorBlockIntegration : ILavalinkIntegration, ISponsorBlockIntegration
{
    private readonly ConcurrentDictionary<ulong, ISkipCategories> _skipCategories;

    public SponsorBlockIntegration()
    {
        _skipCategories = new ConcurrentDictionary<ulong, ISkipCategories>();
    }

    public event AsyncEventHandler<SegmentSkippedEventArgs>? SegmentSkipped;

    public event AsyncEventHandler<SegmentsLoadedEventArgs>? SegmentsLoaded;

    public ImmutableArray<SegmentCategory> DefaultSkipCategories { get; set; }

    public ISkipCategories GetCategories(ulong guildId)
    {
        return _skipCategories.GetOrAdd(guildId, _ => new SkipCategoriesCollection(this));
    }

    /// <inheritdoc/>
    public ValueTask InterceptPayloadAsync(JsonNode jsonNode, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var opCodeNode = jsonNode["op"];
        var guildIdNode = jsonNode["guildId"];

        if (opCodeNode is null || guildIdNode is null || !opCodeNode.GetValue<string>().Equals(OpCode.PlayerPlay.Value, StringComparison.Ordinal))
        {
            return default;
        }

        var guildId = ulong.Parse(guildIdNode.GetValue<string>(), CultureInfo.InvariantCulture);

        var skipCategories = _skipCategories.TryGetValue(guildId, out var guildSkipCategories)
            ? guildSkipCategories.Resolve()
            : DefaultSkipCategories;

        if (skipCategories.IsDefaultOrEmpty)
        {
            return default;
        }

        jsonNode["skipSegments"] = JsonSerializer.SerializeToNode(skipCategories);

        return default;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> ProcessPayloadAsync(PayloadContext payloadContext, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!payloadContext.IsEvent || payloadContext.GuildId is null)
        {
            return false;
        }

        if (payloadContext.EventType!.Value.Value.Equals("SegmentsLoaded", StringComparison.OrdinalIgnoreCase))
        {
            var segmentsLoadedEvent = payloadContext.DeserializeAs<SegmentsLoadedEvent>();

            var eventArgs = new SegmentsLoadedEventArgs(
                guildId: payloadContext.GuildId.Value,
                player: payloadContext.AssociatedPlayer,
                segments: segmentsLoadedEvent.Segments);

            await OnSegmentsLoadedAsync(eventArgs).ConfigureAwait(false);

            return true;
        }

        if (payloadContext.EventType!.Value.Value.Equals("SegmentSkipped", StringComparison.OrdinalIgnoreCase))
        {
            var segmentSkippedEvent = payloadContext.DeserializeAs<SegmentSkippedEvent>();

            var eventArgs = new SegmentSkippedEventArgs(
                guildId: payloadContext.GuildId.Value,
                player: payloadContext.AssociatedPlayer,
                skippedSegment: segmentSkippedEvent.Segment);

            await OnSegmentSkippedAsync(eventArgs).ConfigureAwait(false);

            return true;
        }

        return false;
    }

    private Task OnSegmentSkippedAsync(SegmentSkippedEventArgs eventArgs)
        => SegmentSkipped.InvokeAsync(this, eventArgs);

    private Task OnSegmentsLoadedAsync(SegmentsLoadedEventArgs eventArgs)
        => SegmentsLoaded.InvokeAsync(this, eventArgs);
}
