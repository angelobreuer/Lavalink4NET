namespace Lavalink4NET.Payloads.Events;

using System.Text.Json.Serialization;

/// <summary>
///     The strongly-typed representation of a track exception event received from the lavalink
///     node (in serialized JSON format). For more reference see https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md
/// </summary>
public sealed class TrackExceptionEvent
{
    /// <summary>
    ///     Gets the identifier of the track where the exception occurred.
    /// </summary>
    [JsonPropertyName("track")]
    public string TrackIdentifier { get; init; } = null!;

    [JsonPropertyName("error")]
    public string ErrorMessage { get; init; } = null!;
}
