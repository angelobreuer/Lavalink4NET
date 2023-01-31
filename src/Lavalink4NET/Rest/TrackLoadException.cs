namespace Lavalink4NET.Rest;

using System.Text.Json.Serialization;

public sealed class TrackLoadException
{
    [JsonPropertyName("message")]
    public string ErrorMessage { get; init; }

    [JsonPropertyName("severity")]
    public string Severity { get; init; }
}
