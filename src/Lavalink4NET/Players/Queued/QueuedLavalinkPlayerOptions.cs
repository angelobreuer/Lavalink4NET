namespace Lavalink4NET.Players.Queued;

public record class QueuedLavalinkPlayerOptions : LavalinkPlayerOptions
{
    public int InitialCapacity { get; init; } = 5;

    public int HistoryCapacity { get; init; } = 8;
}
