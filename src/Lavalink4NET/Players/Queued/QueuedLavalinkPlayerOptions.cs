namespace Lavalink4NET.Players.Queued;

public record class QueuedLavalinkPlayerOptions : LavalinkPlayerOptions
{
    public int? HistoryCapacity { get; init; } = 8;

    public bool ClearQueueOnStop { get; init; } = true;

    public bool ClearHistoryOnStop { get; init; } = true;

    public bool ResetTrackRepeatOnStop { get; init; } = true;

    public bool ResetShuffleOnStop { get; init; } = true;

    public TrackRepeatMode DefaultTrackRepeatMode { get; init; } = TrackRepeatMode.None;
}
