namespace Lavalink4NET.InactivityTracking;

using System.Threading.Tasks;
using Lavalink4NET.InactivityTracking.Trackers;

public interface IInactivityTracker
{
    ValueTask<PlayerActivityStatus> CheckAsync(InactivityTrackingContext context, CancellationToken cancellationToken = default);
}
