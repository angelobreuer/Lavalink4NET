namespace Lavalink4NET.InactivityTracking.Trackers.Idle;

using System.Collections.Immutable;
using Lavalink4NET.Players;

public sealed record class IdleInactivityTrackerOptions
{
	public string? Label { get; set; }

	public TimeSpan? Timeout { get; set; }

	public ImmutableArray<PlayerState> IdleStates { get; set; } = IdleInactivityTrackerOptionsDefaults.DefaultIdleStates;
}

file static class IdleInactivityTrackerOptionsDefaults
{
	public static readonly ImmutableArray<PlayerState> DefaultIdleStates = ImmutableArray.Create(
		item1: PlayerState.NotPlaying,
		item2: PlayerState.Paused);
}