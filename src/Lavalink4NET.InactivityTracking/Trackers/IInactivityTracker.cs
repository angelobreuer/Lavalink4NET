namespace Lavalink4NET.InactivityTracking;

using Lavalink4NET.InactivityTracking.Trackers;

public interface IInactivityTracker
{
    InactivityTrackerOptions Options { get; }

    ValueTask RunAsync(
        IInactivityTrackerContext trackerContext,
        CancellationToken cancellationToken = default);
}
