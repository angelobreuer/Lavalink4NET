namespace Lavalink4NET.Protocol.Payloads.Events;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

public sealed record class TrackExceptionEventPayload(
    [property: JsonRequired]
    [property: JsonPropertyName("guildId")]
    [property: JsonConverter(typeof(SnowflakeJsonConverter))]
    ulong GuildId,

    [property: JsonRequired]
    [property: JsonPropertyName("encodedTrack")]
    string TrackData,

    [property: JsonRequired]
    [property: JsonPropertyName("exception")]
    TrackExceptionModel Exception) : IEventPayload;
