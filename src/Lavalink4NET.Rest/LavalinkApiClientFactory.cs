namespace Lavalink4NET.Rest;

using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class LavalinkApiClientFactory : ILavalinkApiClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<LavalinkApiClient> _logger;

    public LavalinkApiClientFactory(
        IHttpClientFactory httpClientFactory,
        IMemoryCache memoryCache,
        ILogger<LavalinkApiClient> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(memoryCache);
        ArgumentNullException.ThrowIfNull(logger);

        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public ILavalinkApiClient Create(IOptions<LavalinkApiClientOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return new LavalinkApiClient(_httpClientFactory, options, _memoryCache, _logger);
    }
}
