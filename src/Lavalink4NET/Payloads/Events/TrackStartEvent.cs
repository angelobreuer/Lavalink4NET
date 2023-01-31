namespace Lavalink4NET.Payloads.Events;

using System.Text.Json.Serialization;

/// <summary>
///     The strongly-typed representation of a track start event received from the lavalink node
///     (in serialized JSON format). For more reference see https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md
/// </summary>
public sealed class TrackStartEvent
{
    /// <summary>
    ///     Gets the identifier of the track that started.
    /// </summary>
    [JsonPropertyName("track")]
    public string TrackIdentifier { get; init; } = string.Empty;
}
