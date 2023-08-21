namespace Lavalink4NET.Protocol.Payloads;

using System.Text.Json.Serialization;

public sealed record class ReadyPayload(
    [property: JsonRequired]
    [property: JsonPropertyName("resumed")]
    bool SessionResumed,

    [property: JsonRequired]
    [property: JsonPropertyName("sessionId")]
    string SessionId) : IPayload;