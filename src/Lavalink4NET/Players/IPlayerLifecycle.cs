namespace Lavalink4NET.Players;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface IPlayerLifecycle : IAsyncDisposable
{
	ValueTask NotifyPlayerCreatedAsync(CancellationToken cancellationToken = default);

	ValueTask NotifyStateChangedAsync(PlayerState playerState, CancellationToken cancellationToken = default);
}
