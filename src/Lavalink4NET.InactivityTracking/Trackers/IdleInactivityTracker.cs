namespace Lavalink4NET.InactivityTracking.Trackers;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players;

public sealed class IdleInactivityTracker : IInactivityTracker
{
    public ValueTask<PlayerActivityStatus> CheckAsync(InactivityTrackingContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var status = context.Player.State is not PlayerState.Playing
            ? PlayerActivityStatus.Inactive
            : PlayerActivityStatus.Active;

        return new ValueTask<PlayerActivityStatus>(status);
    }
}
