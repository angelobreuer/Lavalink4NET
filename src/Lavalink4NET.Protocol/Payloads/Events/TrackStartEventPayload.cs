namespace Lavalink4NET.Protocol.Payloads.Events;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;
using Lavalink4NET.Protocol.Models;

public sealed record class TrackStartEventPayload(
    [property: JsonRequired]
    [property: JsonPropertyName("guildId")]
    [property: JsonConverter(typeof(SnowflakeJsonConverter))]
    ulong GuildId,

    [property: JsonRequired]
    [property: JsonPropertyName("track")]
    TrackModel Track) : IEventPayload;
