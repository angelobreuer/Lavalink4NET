namespace Lavalink4NET.InactivityTracking.Trackers.Idle;

using System.Collections.Immutable;
using Lavalink4NET.Players;

public sealed record class IdleInactivityTrackerOptions
{
    public string? Label { get; set; }

    public TimeSpan? Timeout { get; set; }

    /// <summary>
    ///     Gets or sets the states that are considered as idle.
    /// </summary>
    public ImmutableArray<PlayerState> IdleStates { get; set; } = IdleInactivityTrackerOptionsDefaults.DefaultIdleStates;

    /// <summary>
    ///     Gets or sets the initial timeout for new players.
    /// </summary>
    /// <remarks>
    ///     If <see cref="InitialTimeout"/> is set, <see cref="InitialTimeout"/> will be used as the timeout for new players. Otherwise, <see cref="Timeout"/> will be used.
    ///     If <see cref="InitialTimeout"/> are <see cref="Timeout"/> are both <see langword="null"/>, the default timeout will be used.
    /// </remarks>
    public TimeSpan? InitialTimeout { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether new players should be considered as idle when their initial state is in <see cref="IdleStates"/>. Default is <see langword="true"/>.
    /// </summary>
    public bool TrackNewPlayers { get; set; } = true;
}

file static class IdleInactivityTrackerOptionsDefaults
{
    public static readonly ImmutableArray<PlayerState> DefaultIdleStates = ImmutableArray.Create(
        item1: PlayerState.NotPlaying,
        item2: PlayerState.Paused);
}