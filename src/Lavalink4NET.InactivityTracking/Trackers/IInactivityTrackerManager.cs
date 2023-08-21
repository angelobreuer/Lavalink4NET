namespace Lavalink4NET.InactivityTracking.Trackers;

using System.Collections.Immutable;

public interface IInactivityTrackerManager
{
    ImmutableArray<IInactivityTracker> Trackers { get; }
}
