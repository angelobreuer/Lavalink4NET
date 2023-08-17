namespace Lavalink4NET.InactivityTracking.Trackers;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players;

public sealed class IdleInactivityTracker : IInactivityTracker
{
    private readonly IdleInactivityTrackerOptions _options;

    public IdleInactivityTracker(IdleInactivityTrackerOptions options)
    {
        _options = options;
    }

    public ValueTask<PlayerActivityResult> CheckAsync(InactivityTrackingContext context, ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (player.State is not PlayerState.Playing)
        {
            return new ValueTask<PlayerActivityResult>(PlayerActivityResult.Inactive(_options.Timeout));
        }

        return new ValueTask<PlayerActivityResult>(PlayerActivityResult.Active);
    }
}
