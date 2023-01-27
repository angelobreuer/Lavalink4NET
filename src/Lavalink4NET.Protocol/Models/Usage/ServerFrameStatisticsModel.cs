namespace Lavalink4NET.Protocol.Models.Usage;

using System.Text.Json.Serialization;

public sealed record ServerFrameStatisticsModel(
    [property: JsonRequired]
    [property: JsonPropertyName("sent")]
    int SentFrames,

    [property: JsonRequired]
    [property: JsonPropertyName("sent")]
    int NulledFrames,

    [property: JsonRequired]
    [property: JsonPropertyName("deficit")]
    int DeficitFrames);