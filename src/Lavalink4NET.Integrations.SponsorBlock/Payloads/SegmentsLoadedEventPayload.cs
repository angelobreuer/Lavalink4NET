namespace Lavalink4NET.Integrations.SponsorBlock.Payloads;

using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.SponsorBlock;
using Lavalink4NET.Protocol.Converters;
using Lavalink4NET.Protocol.Payloads;

public sealed record class SegmentsLoadedEventPayload(
    [property: JsonRequired]
    [property: JsonPropertyName("guildId")]
    [property: JsonConverter(typeof(SnowflakeJsonConverter))]
    ulong GuildId,

    [property: JsonRequired]
    [property: JsonPropertyName("segments")]
    ImmutableArray<Segment> Segments) : IEventPayload;
