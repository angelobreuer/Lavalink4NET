namespace Lavalink4NET.Integrations.SponsorBlock.Payloads;

using System;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

public sealed record class SegmentModel(
#if NET7_0_OR_GREATER
    [property: JsonRequired]
#endif
    [property: JsonPropertyName("category")]
    SegmentCategory Category,

#if NET7_0_OR_GREATER
    [property: JsonRequired]
#endif
    [property: JsonPropertyName("start")]
    [property: JsonConverter(typeof(DurationJsonConverter))]
    TimeSpan StartOffset,

#if NET7_0_OR_GREATER
    [property: JsonRequired]
#endif
    [property: JsonPropertyName("end")]
    [property: JsonConverter(typeof(DurationJsonConverter))]
    TimeSpan EndOffset);