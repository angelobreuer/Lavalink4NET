namespace Lavalink4NET.Integrations.SponsorBlock.Payloads;

using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.SponsorBlock;
using Lavalink4NET.Protocol.Converters;
using Lavalink4NET.Protocol.Payloads;

public sealed record class SegmentsLoadedEventPayload(
#if NET7_0_OR_GREATER
    [property: JsonRequired]
#endif
    [property: JsonPropertyName("guildId")]
    [property: JsonConverter(typeof(SnowflakeJsonConverter))]
    ulong GuildId,

#if NET7_0_OR_GREATER
    [property: JsonRequired]
#endif
    [property: JsonPropertyName("segments")]
    ImmutableArray<Segment> Segments) : IEventPayload;
