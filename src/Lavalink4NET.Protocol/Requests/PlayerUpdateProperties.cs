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
    public Optional<string?> TrackData { get; set; }

    [JsonPropertyName("identifier")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<string?> Identifier { get; set; }

    [JsonPropertyName("position")]
    [JsonConverter(typeof(DurationJsonConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<TimeSpan> Position { get; set; }

    [JsonPropertyName("endTime")]
    [JsonConverter(typeof(DurationJsonConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<TimeSpan> EndTime { get; set; }

    [JsonPropertyName("volume")]
    [JsonConverter(typeof(VolumeJsonConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<float> Volume { get; set; }

    [JsonPropertyName("paused")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<bool> IsPaused { get; set; }

    [JsonPropertyName("filters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<IDictionary<string, JsonObject>> Filters { get; set; }

    [JsonPropertyName("voice")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Optional<VoiceStateProperties> VoiceState { get; set; }
}