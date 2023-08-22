namespace Lavalink4NET.Rest;

public readonly record struct LavalinkApiResolutionScope(ILavalinkApiClient? ApiClient = null)
{
    public ValueTask<ILavalinkApiClient> GetClientAsync(ILavalinkApiClientProvider apiClientProvider, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(apiClientProvider);
        return ApiClient is null ? apiClientProvider.GetClientAsync(cancellationToken) : ValueTask.FromResult(ApiClient);
    }
}
