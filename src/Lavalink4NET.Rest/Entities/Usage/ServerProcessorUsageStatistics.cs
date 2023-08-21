namespace Lavalink4NET.Rest.Entities.Usage;

public readonly record struct ServerProcessorUsageStatistics(
    int CoreCount,
    float SystemLoad,
    float LavalinkLoad);