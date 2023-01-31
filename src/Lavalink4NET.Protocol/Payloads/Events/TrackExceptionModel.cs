namespace Lavalink4NET.Protocol.Payloads.Events;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

public sealed record class TrackExceptionModel(
    [property: JsonRequired]
    [property: JsonPropertyName("guildId")]
    [property: JsonConverter(typeof(SnowflakeJsonConverter))]
    ulong GuildId,

    [property: JsonPropertyName("message")]
    string Message,

    [property: JsonPropertyName("severity")]
    string Severity, // TODO

    [property: JsonPropertyName("cause")]
    string Cause);
