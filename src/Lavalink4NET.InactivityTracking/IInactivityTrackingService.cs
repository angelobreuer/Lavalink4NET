namespace Lavalink4NET.InactivityTracking;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Events;
using Lavalink4NET.InactivityTracking.Events;
using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;

public interface IInactivityTrackingService
{
    InactivityTrackingState State { get; }

    event AsyncEventHandler<InactivePlayerEventArgs>? InactivePlayer;

    event AsyncEventHandler<TrackingStatusChangedEventArgs>? TrackingStatusChanged;

    ValueTask<PlayerTrackingState> GetPlayerAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default);

    ValueTask StartAsync(CancellationToken cancellationToken = default);

    ValueTask StopAsync(CancellationToken cancellationToken = default);

    ValueTask PauseAsync(CancellationToken cancellationToken = default);

    ValueTask ResumeAsync(CancellationToken cancellationToken = default);

    ValueTask RunAsync(CancellationToken cancellationToken = default);

    ValueTask PollAsync(CancellationToken cancellationToken = default);
}