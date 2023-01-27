namespace Lavalink4NET.Protocol.Models.Usage;

using System.Text.Json.Serialization;

public sealed record class ServerProcessorUsageStatisticsModel(
    [property: JsonRequired]
    [property: JsonPropertyName("cores")]
    int CoreCount,

    [property: JsonRequired]
    [property: JsonPropertyName("systemLoad")]
    float SystemLoad,

    [property: JsonRequired]
    [property: JsonPropertyName("lavalinkLoad")]
    float LavalinkLoad);
