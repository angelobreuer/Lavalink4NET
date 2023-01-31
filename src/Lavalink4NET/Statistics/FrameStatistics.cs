namespace Lavalink4NET.Statistics;

using System.Text.Json.Serialization;

/// <summary>
///     The frame statistics of a lavalink node.
/// </summary>
public sealed class FrameStatistics
{
    /// <summary>
    ///     Gets the number of average frames sent per minute.
    /// </summary>
    [JsonPropertyName("sent")]
    public int AverageFramesSent { get; init; }

    /// <summary>
    ///     Gets the number of average nulled frames per minute.
    /// </summary>
    [JsonPropertyName("nulled")]
    public int AverageNulledFrames { get; init; }

    /// <summary>
    ///     Gets the number of average deficit frames per minute.
    /// </summary>
    [JsonPropertyName("deficit")]
    public int AverageDeficitFrames { get; init; }
}
