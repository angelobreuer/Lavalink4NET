namespace Lavalink4NET.InactivityTracking.Trackers;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players;

public sealed class IdleInactivityTracker : IInactivityTracker
{
    public ValueTask<bool> CheckAsync(InactivityTrackingContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var isInactive = context.Player.State is not PlayerState.Playing;
        return new ValueTask<bool>(isInactive);
    }
}
