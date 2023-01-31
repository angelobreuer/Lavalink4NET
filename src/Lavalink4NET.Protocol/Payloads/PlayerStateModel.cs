namespace Lavalink4NET.Protocol.Payloads;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

public sealed record class PlayerStateModel(
    [property: JsonRequired]
    [property: JsonPropertyName("time")]
    [property: JsonConverter(typeof(UnixTimestampJsonConverter))]
    DateTimeOffset AbsoluteTimestamp,

    [property: JsonPropertyName("position")]
    [property: JsonConverter(typeof(NullableDurationJsonConverter))]
    TimeSpan? Position,

    [property: JsonRequired]
    [property: JsonPropertyName("connected")]
    bool IsConnected,

    [property: JsonRequired]
    [property: JsonPropertyName("ping")]
    [property: JsonConverter(typeof(NullableDurationJsonConverter))]
    TimeSpan? Latency);