namespace Lavalink4NET.Protocol.Models;

using System.Text.Json.Serialization;

public sealed record class VoiceStateModel(
    [property: JsonRequired]
    [property: JsonPropertyName("token")]
    string Token,

    [property: JsonRequired]
    [property: JsonPropertyName("endpoint")]
    string Endpoint,

    [property: JsonRequired]
    [property: JsonPropertyName("sessionId")]
    string SessionId);
