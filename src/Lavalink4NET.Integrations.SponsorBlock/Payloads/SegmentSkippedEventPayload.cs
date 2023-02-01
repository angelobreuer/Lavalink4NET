namespace Lavalink4NET.Integrations.SponsorBlock.Payloads;

using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.SponsorBlock;
using Lavalink4NET.Protocol.Converters;
using Lavalink4NET.Protocol.Payloads;

public sealed record class SegmentSkippedEventPayload(
    [property: JsonRequired]
    [property: JsonPropertyName("guildId")]
    [property: JsonConverter(typeof(SnowflakeJsonConverter))]
    ulong GuildId,

    [property: JsonRequired]
    [property: JsonPropertyName("segment")]
    Segment Segment) : IEventPayload;