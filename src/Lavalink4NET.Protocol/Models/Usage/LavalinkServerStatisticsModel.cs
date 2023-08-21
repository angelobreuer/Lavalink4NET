namespace Lavalink4NET.Protocol.Models.Usage;

using System;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

public sealed record class LavalinkServerStatisticsModel(
    [property: JsonRequired]
    [property: JsonPropertyName("players")]
    int ConnectedPlayers,

    [property: JsonRequired]
    [property: JsonPropertyName("playingPlayers")]
    int PlayingPlayers,

    [property: JsonRequired]
    [property: JsonPropertyName("uptime")]
    [property: JsonConverter(typeof(DurationJsonConverter))]
    TimeSpan Uptime,

    [property: JsonRequired]
    [property: JsonPropertyName("memory")]
    ServerMemoryUsageStatisticsModel MemoryUsage,

    [property: JsonRequired]
    [property: JsonPropertyName("cpu")]
    ServerProcessorUsageStatisticsModel ProcessorUsage,

    [property: JsonPropertyName("frameStats")]
    ServerFrameStatisticsModel? FrameStatistics);
