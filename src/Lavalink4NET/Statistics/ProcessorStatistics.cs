namespace Lavalink4NET.Statistics;

using System.Text.Json.Serialization;

/// <summary>
///     A wrapper for the statistics.processor object in the statistics update from the lavalink server.
/// </summary>
public sealed class ProcessorStatistics
{
    /// <summary>
    ///     The number of cores the system has.
    /// </summary>
    [JsonPropertyName("cores")]
    public int Cores { get; init; }

    /// <summary>
    ///     The system load (percentage).
    /// </summary>
    [JsonPropertyName("systemLoad")]
    public double SystemLoad { get; init; }

    /// <summary>
    ///     The node load (percentage).
    /// </summary>
    [JsonPropertyName("lavalinkLoad")]
    public double NodeLoad { get; init; }
}
