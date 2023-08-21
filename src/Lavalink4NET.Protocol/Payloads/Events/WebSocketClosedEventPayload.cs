namespace Lavalink4NET.Protocol.Payloads.Events;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

public sealed record class WebSocketClosedEventPayload(
    [property: JsonRequired]
    [property: JsonPropertyName("guildId")]
    [property: JsonConverter(typeof(SnowflakeJsonConverter))]
    ulong GuildId,

    [property: JsonRequired]
    [property: JsonPropertyName("code")]
    int Code,

    [property: JsonRequired]
    [property: JsonPropertyName("reason")]
    string Reason,

    [property: JsonRequired]
    [property: JsonPropertyName("byRemote")]
    bool WasByRemote) : IEventPayload;