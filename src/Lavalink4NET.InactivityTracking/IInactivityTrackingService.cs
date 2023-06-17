namespace Lavalink4NET.InactivityTracking;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Events;
using Lavalink4NET.InactivityTracking.Events;
using Lavalink4NET.Players;
using Lavalink4NET.Tracking;

public interface IInactivityTrackingService
{
    InactivityTrackingState State { get; }

    event AsyncEventHandler<InactivePlayerEventArgs>? InactivePlayer;

    event AsyncEventHandler<PlayerTrackingStatusUpdateEventArgs>? PlayerTrackingStatusUpdated;

    InactivityTrackingStatus GetStatus(ILavalinkPlayer player);

    ValueTask StartAsync(CancellationToken cancellationToken = default);

    ValueTask StopAsync(CancellationToken cancellationToken = default);

    ValueTask PauseAsync(CancellationToken cancellationToken = default);

    ValueTask ResumeAsync(CancellationToken cancellationToken = default);

    ValueTask RunAsync(CancellationToken cancellationToken = default);

    ValueTask PollAsync(CancellationToken cancellationToken = default);

    ValueTask NotifyAsync(ulong guildId, ILavalinkPlayer? player, CancellationToken cancellationToken = default);
}