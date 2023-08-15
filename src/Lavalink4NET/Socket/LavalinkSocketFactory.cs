namespace Lavalink4NET.Socket;

using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class LavalinkSocketFactory : ILavalinkSocketFactory
{
    private readonly IHttpMessageHandlerFactory _httpMessageHandlerFactory;
    private readonly ILogger<LavalinkSocket> _logger;

    public LavalinkSocketFactory(IHttpMessageHandlerFactory httpMessageHandlerFactory, ILogger<LavalinkSocket> logger)
    {
        ArgumentNullException.ThrowIfNull(httpMessageHandlerFactory);
        ArgumentNullException.ThrowIfNull(logger);

        _httpMessageHandlerFactory = httpMessageHandlerFactory;
        _logger = logger;
    }

    public ILavalinkSocket Create(IOptions<LavalinkSocketOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return new LavalinkSocket(_httpMessageHandlerFactory, _logger, options);
    }
}
