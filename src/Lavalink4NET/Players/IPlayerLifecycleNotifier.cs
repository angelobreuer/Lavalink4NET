namespace Lavalink4NET.Players;

using System.Threading;
using System.Threading.Tasks;

internal interface IPlayerLifecycleNotifier
{
    ValueTask NotifyDisposeAsync(ulong guildId, CancellationToken cancellationToken = default);
}
