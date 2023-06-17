namespace Lavalink4NET.InactivityTracking;

using System.Threading.Tasks;

public interface IInactivityTracker
{
    ValueTask<bool> CheckAsync(InactivityTrackingContext context, CancellationToken cancellationToken = default);
}
