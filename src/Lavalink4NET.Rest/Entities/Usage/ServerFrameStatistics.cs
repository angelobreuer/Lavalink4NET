namespace Lavalink4NET.Rest.Entities.Usage;

public readonly record struct ServerFrameStatistics(
    int SentFrames,
    int NulledFrames,
    int DeficitFrames);
