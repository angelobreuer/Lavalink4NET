namespace Lavalink4NET.Players.Queued;

public record class QueuedLavalinkPlayerOptions : LavalinkPlayerOptions
{
    /// <summary>
    ///     If set, the player will use this queue instead of creating a new one.
    /// </summary>
    public ITrackQueue? TrackQueue { get; init; }

    public int? HistoryCapacity { get; init; } = 8;

    public bool EnableAutoPlay { get; init; } = true;

    public bool ClearQueueOnStop { get; init; } = true;

    public bool ClearHistoryOnStop { get; init; } = false;

    public bool ResetTrackRepeatOnStop { get; init; } = true;

    public bool ResetShuffleOnStop { get; init; } = true;

    /// <summary>
    ///     Denotes whether to respect the track repeat mode when skipping tracks. For example, if the track repeat mode is set to
    ///     <see cref="TrackRepeatMode.Track" /> and the current track is skipped, the player will repeat the current track. If this
    ///     property is set to <see langword="false" />, the player will not repeat the current track. The default value is
    ///     <see langword="false" />.
    /// </summary>
    public bool RespectTrackRepeatOnSkip { get; init; } = false;

    public TrackRepeatMode DefaultTrackRepeatMode { get; init; } = TrackRepeatMode.None;
}
