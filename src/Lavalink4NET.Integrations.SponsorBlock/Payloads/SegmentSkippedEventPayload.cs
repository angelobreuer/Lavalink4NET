namespace Lavalink4NET.Integrations.SponsorBlock.Payloads;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;
using Lavalink4NET.Protocol.Payloads;

public sealed record class SegmentSkippedEventPayload(
#if NET7_0_OR_GREATER
    [property: JsonRequired]
#endif
    [property: JsonPropertyName("guildId")]
    [property: JsonConverter(typeof(SnowflakeJsonConverter))]
    ulong GuildId,

#if NET7_0_OR_GREATER
    [property: JsonRequired]
#endif
    [property: JsonPropertyName("segment")]
    SegmentModel Segment) : IEventPayload;