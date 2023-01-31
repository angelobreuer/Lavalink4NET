namespace Lavalink4NET.Payloads.Node;

using System;
using System.Text.Json.Serialization;
using Lavalink4NET.Converters;
using Statistics;

/// <summary>
///     The strongly-typed representation of a node statistics payload received from the
///     lavalink node (in serialized JSON format). For more reference see https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md
/// </summary>
public sealed class StatsUpdatePayload
{
    /// <summary>
    ///     Gets the number of players the node is holding.
    /// </summary>
    [JsonPropertyName("players")]
    public int Players { get; init; }

    /// <summary>
    ///     Gets the number of players that are currently playing using the node.
    /// </summary>
    [JsonPropertyName("playingPlayers")]
    public int PlayingPlayers { get; init; }

    /// <summary>
    ///     Gets the uptime from the node (how long the node is online).
    /// </summary>
    [JsonConverter(typeof(TimeSpanJsonConverter))]
    [JsonPropertyName("uptime")]
    public TimeSpan Uptime { get; init; }

    /// <summary>
    ///     Gets usage statistics for the memory of the node.
    /// </summary>
    [JsonPropertyName("memory")]
    public MemoryStatistics Memory { get; init; } = null!;

    /// <summary>
    ///     Gets usage statistics for the processor of the node.
    /// </summary>
    [JsonPropertyName("cpu")]
    public ProcessorStatistics Processor { get; init; } = null!;

    /// <summary>
    ///     Gets frame statistics of the node.
    /// </summary>
    [JsonPropertyName("frameStats")]
    public FrameStatistics FrameStatistics { get; init; } = null!;
}
