namespace Lavalink4NET.Integrations.SponsorBlock.Payloads;

using System;
using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.SponsorBlock.Converters;

public sealed record class ChapterModel(
#if NET7_0_OR_GREATER
    [property: JsonRequired]
#endif
    [property: JsonPropertyName("name")]
    string Name,

#if NET7_0_OR_GREATER
    [property: JsonRequired]
#endif
    [property: JsonConverter(typeof(NumberTimeSpanJsonConverter))]
    [property: JsonPropertyName("start")]
    TimeSpan Start,

#if NET7_0_OR_GREATER
    [property: JsonRequired]
#endif
    [property: JsonConverter(typeof(NumberTimeSpanJsonConverter))]
    [property: JsonPropertyName("end")]
    TimeSpan End,

#if NET7_0_OR_GREATER
    [property: JsonRequired]
#endif
    [property: JsonConverter(typeof(NumberTimeSpanJsonConverter))]
    [property: JsonPropertyName("duration")]
    TimeSpan Duration);