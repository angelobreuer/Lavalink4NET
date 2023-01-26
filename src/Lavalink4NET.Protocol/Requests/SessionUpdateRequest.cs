namespace Lavalink4NET.Protocol.Requests;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

public sealed record class SessionUpdateRequest(
    [property: JsonPropertyName("resumingKey")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    Optional<string?> ResumingKey,

    [property: JsonPropertyName("timeout")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [property: JsonConverter<DurationJsonConverter>]
    Optional<TimeSpan> Timeout);