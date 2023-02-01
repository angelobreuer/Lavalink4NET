namespace Lavalink4NET.Integrations.SponsorBlock.Payloads;

using System;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

public sealed record class SegmentModel(
    [property: JsonRequired]
    [property: JsonPropertyName("category")]
    SegmentCategory Category,

    [property: JsonRequired]
    [property: JsonPropertyName("start")]
    [property: JsonConverter(typeof(DurationJsonConverter))]
    TimeSpan StartOffset,

    [property: JsonRequired]
    [property: JsonPropertyName("end")]
    [property: JsonConverter(typeof(DurationJsonConverter))]
    TimeSpan EndOffset);