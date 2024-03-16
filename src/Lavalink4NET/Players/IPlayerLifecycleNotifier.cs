namespace Lavalink4NET.Players;

using System.Threading;
using System.Threading.Tasks;

internal interface IPlayerLifecycleNotifier
{
    ValueTask NotifyPlayerCreatedAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default);

    ValueTask NotifyDisposeAsync(ulong guildId, CancellationToken cancellationToken = default);

    ValueTask NotifyStateChangedAsync(ILavalinkPlayer player, PlayerState playerState, CancellationToken cancellationToken = default);
}
