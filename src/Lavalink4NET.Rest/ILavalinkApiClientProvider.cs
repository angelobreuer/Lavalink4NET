namespace Lavalink4NET.Rest;

public interface ILavalinkApiClientProvider
{
    ValueTask<ILavalinkApiClient> GetClientAsync(CancellationToken cancellationToken = default);
}
