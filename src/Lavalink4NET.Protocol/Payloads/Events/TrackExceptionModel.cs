namespace Lavalink4NET.Protocol.Payloads;

using System.Text.Json.Serialization;

public sealed record class TrackExceptionModel(
    [property: JsonRequired]
    [property: JsonPropertyName("message")]
    string Message,

    [property: JsonRequired]
    [property: JsonPropertyName("severity")]
    string Severity, // TODO

    [property: JsonRequired]
    [property: JsonPropertyName("cause")]
    string Cause);
