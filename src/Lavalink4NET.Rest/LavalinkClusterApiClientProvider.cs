namespace Lavalink4NET.Rest;

public sealed class LavalinkApiClientProvider : ILavalinkApiClientProvider
{
    private readonly ILavalinkApiClient _client;

    public LavalinkApiClientProvider(ILavalinkApiClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        _client = client;
    }

    public ValueTask<ILavalinkApiClient> GetClientAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(_client);
    }
}
