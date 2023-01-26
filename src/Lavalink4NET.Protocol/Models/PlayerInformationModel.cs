namespace Lavalink4NET.Protocol.Models;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

public sealed record class PlayerInformationModel(
    [property: JsonPropertyName("guildId")]
    [property: JsonRequired]
    [property: JsonConverter<SnowflakeJsonConverter>]
    ulong GuildId,

    [property: JsonPropertyName("track")]
    [property: JsonConverter<SnowflakeJsonConverter>]
    TrackModel? CurrentTrack,

    [property: JsonPropertyName("volume")]
    [property: JsonRequired]
    [property: JsonConverter<VolumeJsonConverter>]
    float Volume,

    [property: JsonPropertyName("paused")]
    [property: JsonRequired]
    bool IsPaused); // TODO