namespace Lavalink4NET.Integrations.SponsorBlock;

using System;
using System.Text.Json.Serialization;
using Lavalink4NET.Converters;

public sealed class Segment
{
    [JsonPropertyName("category")]
    public SegmentCategory Category { get; init; }

    [JsonPropertyName("start")]
    [JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan StartOffset { get; init; }

    [JsonPropertyName("end")]
    [JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan EndOffset { get; init; }
}
