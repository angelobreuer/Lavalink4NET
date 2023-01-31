namespace Lavalink4NET.Statistics;

using System;

/// <summary>
///     The statistics for a <see cref="LavalinkNode"/>.
/// </summary>
public sealed class NodeStatistics
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NodeStatistics"/> class.
    /// </summary>
    /// <param name="players">the number of players the node is holding</param>
    /// <param name="playingPlayers">
    ///     the number of players that are currently playing using the node
    /// </param>
    /// <param name="uptime">the uptime from the node (how long the node is online)</param>
    /// <param name="memory">the usage statistics for the memory of the node</param>
    /// <param name="processor">the usage statistics for the processor of the node</param>
    /// <param name="frameStatistics">the frame statistics of the node</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="memory"/> parameter is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="processor"/> parameter is <see langword="null"/>.
    /// </exception>
    public NodeStatistics(int players, int playingPlayers, TimeSpan uptime, MemoryStatistics memory,
        ProcessorStatistics processor, FrameStatistics frameStatistics)
    {
        Players = players;
        PlayingPlayers = playingPlayers;
        Uptime = uptime;
        FrameStatistics = frameStatistics;
        Memory = memory ?? throw new ArgumentNullException(nameof(memory));
        Processor = processor ?? throw new ArgumentNullException(nameof(processor));
    }

    /// <summary>
    ///     Gets the number of players the node is holding.
    /// </summary>
    public int Players { get; init; }

    /// <summary>
    ///     Gets the number of players that are currently playing using the node.
    /// </summary>
    public int PlayingPlayers { get; init; }

    /// <summary>
    ///     Gets the uptime from the node (how long the node is online).
    /// </summary>
    public TimeSpan Uptime { get; init; }

    /// <summary>
    ///     Gets the usage statistics for the memory of the node.
    /// </summary>
    public MemoryStatistics Memory { get; init; }

    /// <summary>
    ///     Gets the usage statistics for the processor of the node.
    /// </summary>
    public ProcessorStatistics Processor { get; init; }

    /// <summary>
    ///     Gets the frame statistics of the node.
    /// </summary>
    public FrameStatistics FrameStatistics { get; init; }
}
