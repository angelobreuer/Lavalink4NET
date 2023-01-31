namespace Lavalink4NET.Payloads.Events;

using System.Text.Json.Serialization;

/// <summary>
///     The strongly-typed representation of a track stuck event received from the lavalink node
///     (in serialized JSON format). For more reference see https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md
/// </summary>
public sealed class TrackStuckEvent
{
    /// <summary>
    ///     Gets the identifier of the track that got stuck.
    /// </summary>
    [JsonPropertyName("track")]
    public string TrackIdentifier { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the threshold in milliseconds.
    /// </summary>
    [JsonPropertyName("thresholdMs")]
    public long Threshold { get; init; }
}
