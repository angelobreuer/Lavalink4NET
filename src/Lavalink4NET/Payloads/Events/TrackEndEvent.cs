namespace Lavalink4NET.Payloads.Events;

using System.Text.Json.Serialization;
using Lavalink4NET.Player;

/// <summary>
///     The strongly-typed representation of a track end event received from the lavalink node
///     (in serialized JSON format). For more reference see https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md
/// </summary>
public sealed class TrackEndEvent
{
    /// <summary>
    ///     Gets the identifier of the track that has ended.
    /// </summary>
    [JsonPropertyName("track")]
    public string TrackIdentifier { get; init; } = null!;

    /// <summary>
    ///     Gets the reason why the track ended.
    /// </summary>
    [JsonPropertyName("reason")]
    public TrackEndReason Reason { get; init; }
}
