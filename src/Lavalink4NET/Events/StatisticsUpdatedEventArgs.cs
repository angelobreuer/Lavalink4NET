namespace Lavalink4NET.Events;

using System;
using Lavalink4NET.Rest.Entities.Usage;

public sealed class StatisticsUpdatedEventArgs : EventArgs
{
    public StatisticsUpdatedEventArgs(LavalinkServerStatistics statistics)
    {
        ArgumentNullException.ThrowIfNull(statistics);

        Statistics = statistics;
    }

    /// <summary>
    ///     Gets the statistics for the node.
    /// </summary>
    public LavalinkServerStatistics Statistics { get; }
}
