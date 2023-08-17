namespace Lavalink4NET.InactivityTracking;

using System.Threading.Tasks;
using Lavalink4NET.Players;

public interface IInactivityTracker
{
    ValueTask<PlayerActivityResult> CheckAsync(InactivityTrackingContext context, ILavalinkPlayer player, CancellationToken cancellationToken = default);
}
