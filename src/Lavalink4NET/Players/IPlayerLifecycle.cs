namespace Lavalink4NET.Players;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface IPlayerLifecycle : IAsyncDisposable
{
    ValueTask NotifyPlayerCreatedAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default);

    ValueTask NotifyStateChangedAsync(ILavalinkPlayer player, PlayerState playerState, CancellationToken cancellationToken = default);
}
