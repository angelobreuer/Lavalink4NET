namespace Lavalink4NET.Protocol.Models;

using System;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

public sealed record class VoiceStateModel(
    [property: JsonRequired]
    [property: JsonPropertyName("token")]
    string Token,

    [property: JsonRequired]
    [property: JsonPropertyName("endpoint")]
    string Endpoint,

    [property: JsonRequired]
    [property: JsonPropertyName("sessionId")]
    string SessionId,

    [property: JsonPropertyName("connected")]
    bool? IsConnected,

    [property: JsonPropertyName("ping")]
    [property: JsonConverter(typeof(NullableDurationJsonConverter))]
    TimeSpan? Latency);
