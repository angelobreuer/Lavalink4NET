namespace Lavalink4NET.Payloads.Player;

using System;
using System.Text.Json.Serialization;
using Lavalink4NET.Converters;

/// <summary>
///     A wrapper for the player status object.
/// </summary>
public readonly struct PlayerStatus
{
    /// <summary>
    ///     Gets the track position (at the time the update was received, see: <see cref="UpdateTime"/>).
    /// </summary>
    [JsonPropertyName("position")]
    [JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan Position { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the player is connected.
    /// </summary>
    [JsonPropertyName("connected")]
    public bool IsConnected { get; init; }

    /// <summary>
    ///     Gets the time when the position update was sent.
    /// </summary>
    [JsonPropertyName("time")]
    [JsonConverter(typeof(UnixJsonDateTimeOffsetConverter))]
    public DateTimeOffset UpdateTime { get; init; }
}
