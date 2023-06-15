namespace Lavalink4NET.Players;

using System.Threading;
using System.Threading.Tasks;

public interface ILavalinkSessionProvider
{
    ValueTask<string> GetSessionIdAsync(CancellationToken cancellationToken = default);
}
