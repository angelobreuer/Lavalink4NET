namespace Lavalink4NET.SponsorBlock.Payloads;

using System.Collections.Immutable;
using System.Text.Json.Serialization;

public sealed class SegmentsLoadedEvent
{
    [JsonPropertyName("segments")]
    public ImmutableArray<Segment> Segments { get; init; }
}
