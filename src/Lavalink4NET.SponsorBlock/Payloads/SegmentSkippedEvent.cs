namespace Lavalink4NET.SponsorBlock.Payloads;

using System.Text.Json.Serialization;

public sealed class SegmentSkippedEvent
{
    [JsonPropertyName("segment")]
    public Segment Segment { get; init; }
}
