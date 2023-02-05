namespace Lavalink4NET.Events;

using System;
using Lavalink4NET.Rest.Entities.Usage;

/// <summary>
///     The event arguments for the <see cref="LavalinkNode.StatisticsUpdated"/> event.
/// </summary>
public sealed class NodeStatisticsUpdateEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the No
    /// </summary>
    /// <param name="statistics">the statistics for the node</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="statistics"/> parameter is <see langword="null"/>.
    /// </exception>
    public NodeStatisticsUpdateEventArgs(LavalinkServerStatistics statistics)
    {
        ArgumentNullException.ThrowIfNull(statistics);

        Statistics = statistics;
    }

    /// <summary>
    ///     Gets the statistics for the node.
    /// </summary>
    public LavalinkServerStatistics Statistics { get; }
}
