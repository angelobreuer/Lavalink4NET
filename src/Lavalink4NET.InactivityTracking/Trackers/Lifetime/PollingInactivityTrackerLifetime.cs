namespace Lavalink4NET.InactivityTracking.Trackers.Lifetime;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

internal sealed class PollingInactivityTrackerLifetime : InactivityTrackerLifetimeBase
{
    private readonly TimeSpan _pollInterval;
    private readonly IInactivityTracker _inactivityTracker;
    private readonly IInactivityTrackerContext _inactivityTrackerContext;

    public PollingInactivityTrackerLifetime(
        string label,
        IInactivityTracker inactivityTracker,
        IInactivityTrackerContext inactivityTrackerContext,
        ILogger<PollingInactivityTrackerLifetime> logger,
        TimeSpan pollInterval)
        : base(label, inactivityTracker, logger)
    {
        ArgumentNullException.ThrowIfNull(inactivityTracker);

        _inactivityTracker = inactivityTracker;
        _inactivityTrackerContext = inactivityTrackerContext;
        _pollInterval = pollInterval;
    }

    protected override async ValueTask RunAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var periodicTimer = new PeriodicTimer(_pollInterval);

        while (await periodicTimer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
        {
            await _inactivityTracker
                .RunAsync(_inactivityTrackerContext, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
