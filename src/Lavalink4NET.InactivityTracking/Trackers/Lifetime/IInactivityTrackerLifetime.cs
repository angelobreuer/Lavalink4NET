namespace Lavalink4NET.InactivityTracking.Trackers.Lifetime;

internal interface IInactivityTrackerLifetime
{
    IInactivityTracker InactivityTracker { get; }

    string Label { get; }

    ValueTask StartAsync(CancellationToken cancellationToken = default);

    ValueTask StopAsync(CancellationToken cancellationToken = default);
}
