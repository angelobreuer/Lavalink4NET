namespace Lavalink4NET.Protocol.Models;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;
using Lavalink4NET.Protocol.Models.Filters;

public sealed record class PlayerInformationModel(
    [property: JsonRequired]
    [property: JsonPropertyName("guildId")]
    [property: JsonConverter(typeof(SnowflakeJsonConverter))]
    ulong GuildId,

    [property: JsonPropertyName("track")]
    TrackModel? CurrentTrack,

    [property: JsonRequired]
    [property: JsonPropertyName("volume")]
    [property: JsonConverter(typeof(VolumeJsonConverter))]
    float Volume,

    [property: JsonRequired]
    [property: JsonPropertyName("paused")]
    bool IsPaused,

    [property: JsonRequired]
    [property: JsonPropertyName("voice")]
    VoiceStateModel VoiceState,

    [property: JsonRequired]
    [property: JsonPropertyName("filters")]
    PlayerFilterMapModel Filters);