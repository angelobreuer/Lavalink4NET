namespace Lavalink4NET.InactivityTracking.Queue;

using Lavalink4NET.Players;

public interface IInactivityExpirationQueue
{
    ValueTask<ILavalinkPlayer?> GetExpiredPlayerAsync(CancellationToken cancellationToken = default);

    bool TryCancel(ILavalinkPlayer player);

    bool TryNotify(ILavalinkPlayer player, DateTimeOffset expireAfter);
}
