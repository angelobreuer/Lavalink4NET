namespace Lavalink4NET.Tracking;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Events;
using Lavalink4NET.InactivityTracking.Events;
using Lavalink4NET.Players;

public interface IInactivityTrackingService
{
    bool IsRunning { get; }

    event AsyncEventHandler<InactivePlayerEventArgs>? InactivePlayer;

    event AsyncEventHandler<PlayerTrackingStatusUpdateEventArgs>? PlayerTrackingStatusUpdated;

    InactivityTrackingStatus GetStatus(ILavalinkPlayer player);

    ValueTask PollAsync(CancellationToken cancellationToken = default);

    void Start();

    void Stop();

    ValueTask UntrackPlayerAsync(ulong guildId, ILavalinkPlayer? player, CancellationToken cancellationToken = default);
}