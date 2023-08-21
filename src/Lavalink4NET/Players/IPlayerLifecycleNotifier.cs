namespace Lavalink4NET.Players;

using System.Threading;
using System.Threading.Tasks;

internal interface IPlayerLifecycleNotifier
{
	ValueTask NotifyPlayerCreatedAsync(ulong guildId, CancellationToken cancellationToken = default);

	ValueTask NotifyDisposeAsync(ulong guildId, CancellationToken cancellationToken = default);

	ValueTask NotifyStateChangedAsync(ulong guildId, PlayerState playerState, CancellationToken cancellationToken = default);
}
