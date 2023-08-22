namespace Lavalink4NET.Rest;

using System.Threading;
using System.Threading.Tasks;

public interface ILavalinkApiClientProvider
{
    ValueTask<ILavalinkApiClient> GetClientAsync(CancellationToken cancellationToken = default);
}
