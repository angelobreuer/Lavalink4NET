namespace Lavalink4NET.Integrations.SponsorBlock.Payloads;

using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.SponsorBlock;

public sealed class SegmentsLoadedEvent
{
    [JsonPropertyName("segments")]
    public ImmutableArray<Segment> Segments { get; init; }
}
