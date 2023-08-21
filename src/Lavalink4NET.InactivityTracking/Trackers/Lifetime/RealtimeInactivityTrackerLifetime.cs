namespace Lavalink4NET.InactivityTracking.Trackers.Lifetime;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

internal sealed class RealtimeInactivityTrackerLifetime : InactivityTrackerLifetimeBase
{
    private readonly IInactivityTracker _inactivityTracker;
    private readonly IInactivityTrackerContext _inactivityTrackerContext;

    public RealtimeInactivityTrackerLifetime(
        string label,
        IInactivityTracker inactivityTracker,
        IInactivityTrackerContext inactivityTrackerContext,
        ILogger<RealtimeInactivityTrackerLifetime> logger)
        : base(label, inactivityTracker, logger)
    {
        ArgumentNullException.ThrowIfNull(inactivityTracker);
        ArgumentNullException.ThrowIfNull(inactivityTrackerContext);

        _inactivityTracker = inactivityTracker;
        _inactivityTrackerContext = inactivityTrackerContext;
    }

    protected override ValueTask RunAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _inactivityTracker.RunAsync(_inactivityTrackerContext, cancellationToken);
    }
}
