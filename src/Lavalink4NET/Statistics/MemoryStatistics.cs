namespace Lavalink4NET.Statistics;

using System.Text.Json.Serialization;

/// <summary>
///     A wrapper for the statistics.memory object in the statistics update from the lavalink server.
/// </summary>
public sealed class MemoryStatistics
{
    /// <summary>
    ///     The free RAM memory in bytes.
    /// </summary>
    [JsonPropertyName("free")]
    public ulong FreeMemory { get; init; }

    /// <summary>
    ///     The used RAM memory in bytes.
    /// </summary>
    [JsonPropertyName("used")]
    public ulong UsedMemory { get; init; }

    /// <summary>
    ///     The allocated RAM memory in bytes.
    /// </summary>
    [JsonPropertyName("allocated")]
    public ulong AllocatedMemory { get; init; }

    /// <summary>
    ///     The reservable RAM memory in bytes.
    /// </summary>
    [JsonPropertyName("reservable")]
    public ulong ReservableMemory { get; init; }
}
