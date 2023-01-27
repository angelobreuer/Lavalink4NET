namespace Lavalink4NET.Protocol.Models.Usage;

using System.Text.Json.Serialization;

public sealed record class ServerMemoryUsageStatisticsModel(
    [property: JsonRequired]
    [property: JsonPropertyName("free")]
    long FreeMemory,

    [property: JsonRequired]
    [property: JsonPropertyName("used")]
    long UsedMemory,

    [property: JsonRequired]
    [property: JsonPropertyName("allocated")]
    long AllocatedMemory,

    [property: JsonRequired]
    [property: JsonPropertyName("reservable")]
    long ReservableMemory);
