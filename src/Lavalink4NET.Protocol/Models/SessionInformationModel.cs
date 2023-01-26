namespace Lavalink4NET.Protocol.Models;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

public sealed record class SessionInformationModel(
    [property: JsonPropertyName("resumingKey")]
    string? ResumingKey,

    [property: JsonPropertyName("timeout")]
    [property: JsonConverter<DurationJsonConverter>]
    TimeSpan Timeout);