namespace Lavalink4NET.InactivityTracking.Players;

using Lavalink4NET.Players;

public interface IInactivityPlayerListener : ILavalinkPlayerListener
{
    ValueTask NotifyPlayerInactiveAsync(CancellationToken cancellationToken = default);

    ValueTask NotifyPlayerActiveAsync(CancellationToken cancellationToken = default);

    ValueTask NotifyPlayerTrackedAsync(CancellationToken cancellationToken = default);
}
