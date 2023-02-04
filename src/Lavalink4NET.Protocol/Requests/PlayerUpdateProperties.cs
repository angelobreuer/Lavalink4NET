namespace Lavalink4NET.Protocol.Requests;

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

public sealed record class PlayerUpdateProperties
{
    [JsonPropertyName("encodedTrack")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(OptionalJsonConverter<string?>))]
    public Optional<string?> TrackData { get; set; }

    [JsonPropertyName("identifier")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(OptionalJsonConverter<string?>))]
    public Optional<string?> Identifier { get; set; }

    [JsonPropertyName("position")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(OptionalJsonConverter<TimeSpan, DurationJsonConverter>))]
    public Optional<TimeSpan> Position { get; set; }

    [JsonPropertyName("endTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(OptionalJsonConverter<TimeSpan, DurationJsonConverter>))]
    public Optional<TimeSpan> EndTime { get; set; }

    [JsonPropertyName("volume")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(OptionalJsonConverter<float, VolumeJsonConverter>))]
    public Optional<float> Volume { get; set; }

    [JsonPropertyName("paused")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(OptionalJsonConverter<bool>))]
    public Optional<bool> IsPaused { get; set; }

    [JsonPropertyName("filters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(OptionalJsonConverter<IDictionary<string, JsonObject>>))]
    public Optional<IDictionary<string, JsonObject>> Filters { get; set; }

    [JsonPropertyName("voice")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(OptionalJsonConverter<VoiceStateProperties>))]
    public Optional<VoiceStateProperties> VoiceState { get; set; }
}