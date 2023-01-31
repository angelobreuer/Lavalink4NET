namespace Lavalink4NET.Payloads.Player;

using System;
using System.Text.Json.Serialization;
using Lavalink4NET.Converters;

/// <summary>
///     The strongly-typed representation of a player play payload sent to the lavalink node (in
///     serialized JSON format). For more reference see https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md
/// </summary>
public sealed class PlayerPlayPayload
{
    /// <summary>
    ///     Gets the guild snowflake identifier the player update is for.
    /// </summary>
    [JsonPropertyName("guildId")]
    [JsonConverter(typeof(UInt64AsStringJsonSerializer))]
    public ulong GuildId { get; init; }

    /// <summary>
    ///     Gets the track identifier that the player should play.
    /// </summary>
    [JsonPropertyName("track")]
    public string TrackIdentifier { get; init; }

    /// <summary>
    ///     Gets the track start position in milliseconds.
    /// </summary>
    [JsonPropertyName("startTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan? StartTime { get; init; }

    /// <summary>
    ///     Gets the track end position in milliseconds.
    /// </summary>
    [JsonPropertyName("endTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan? EndTime { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the track play should be ignored if the same track
    ///     is currently playing.
    /// </summary>
    [JsonPropertyName("noReplace")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool NoReplace { get; init; }
}
