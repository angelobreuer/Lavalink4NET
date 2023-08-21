namespace Lavalink4NET.Players.Preconditions;

using System.Threading;
using System.Threading.Tasks;

public interface IPlayerPrecondition
{
    ValueTask<bool> CheckAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default);
}
