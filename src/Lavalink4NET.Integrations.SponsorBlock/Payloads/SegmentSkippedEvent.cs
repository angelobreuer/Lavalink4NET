namespace Lavalink4NET.Integrations.SponsorBlock.Payloads;

using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.SponsorBlock;

public sealed class SegmentSkippedEvent
{
    [JsonPropertyName("segment")]
    public Segment Segment { get; init; }
}
