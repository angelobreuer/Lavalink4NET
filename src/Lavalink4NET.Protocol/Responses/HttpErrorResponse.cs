namespace Lavalink4NET.Protocol.Responses;

using System.Text.Json.Serialization;

public sealed record class HttpErrorResponse(
    [property: JsonRequired]
    [property: JsonPropertyName("timestamp")]
    [property: JsonConverter(typeof(UnixTimestampJsonConverter))]
    DateTimeOffset Timestamp,

    [property: JsonRequired]
    [property: JsonPropertyName("status")]
    int StatusCode,

    [property: JsonRequired]
    [property: JsonPropertyName("error")]
    string ReasonPhrase,

    [property: JsonPropertyName("trace")]
    string? StackTrace,

    [property: JsonRequired]
    [property: JsonPropertyName("message")]
    string ErrorMessage,

    [property: JsonRequired]
    [property: JsonPropertyName("path")]
    string RequestPath);