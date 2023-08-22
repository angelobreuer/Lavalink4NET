namespace Lavalink4NET;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Rest;
using Microsoft.Extensions.Options;

internal sealed class LavalinkApiClientProvider : ILavalinkApiClientProvider
{
    private readonly ILavalinkApiClientFactory _lavalinkApiClientFactory;
    private readonly IOptions<LavalinkNodeOptions> _options;
    private ILavalinkApiClient? _apiClient;

    public LavalinkApiClientProvider(
        ILavalinkApiClientFactory lavalinkApiClientFactory,
        IOptions<AudioServiceOptions> options)
    {
        ArgumentNullException.ThrowIfNull(lavalinkApiClientFactory);
        ArgumentNullException.ThrowIfNull(options);

        _lavalinkApiClientFactory = lavalinkApiClientFactory;
        _options = options;
    }

    public ValueTask<ILavalinkApiClient> GetClientAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var apiClient = _apiClient ??= _lavalinkApiClientFactory.Create(_options);
        return new ValueTask<ILavalinkApiClient>(apiClient);
    }
}
