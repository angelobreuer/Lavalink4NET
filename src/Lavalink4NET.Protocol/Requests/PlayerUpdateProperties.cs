namespace Lavalink4NET.Protocol.Requests;

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

public sealed record class PlayerUpdateProperties(
    [property: JsonPropertyName("encodedTrack")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    Optional<string?> TrackData,

    [property: JsonPropertyName("identifier")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    Optional<string?> Identifier,

    [property: JsonPropertyName("position")]
    [property: JsonConverter<DurationJsonConverter>]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    Optional<TimeSpan> Position,

    [property: JsonPropertyName("endTime")]
    [property: JsonConverter<DurationJsonConverter>]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    Optional<TimeSpan> EndTime,

    [property: JsonPropertyName("volume")]
    [property: JsonConverter<VolumeJsonConverter>]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    Optional<float> Volume,

    [property: JsonPropertyName("paused")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    Optional<bool> IsPaused,

    [property: JsonPropertyName("filters")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    Optional<IDictionary<string, JsonObject>> Filters,

    [property: JsonPropertyName("voice")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    Optional<VoiceStateProperties> VoiceState);