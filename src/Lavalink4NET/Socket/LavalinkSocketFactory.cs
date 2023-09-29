namespace Lavalink4NET.Socket;

using System;
using System.Net.Http;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class LavalinkSocketFactory : ILavalinkSocketFactory
{
    private readonly IHttpMessageHandlerFactory _httpMessageHandlerFactory;
    private readonly IReconnectStrategy _reconnectStrategy;
    private readonly ISystemClock _systemClock;
    private readonly ILogger<LavalinkSocket> _logger;

    public LavalinkSocketFactory(IHttpMessageHandlerFactory httpMessageHandlerFactory, IReconnectStrategy reconnectStrategy, ISystemClock systemClock, ILogger<LavalinkSocket> logger)
    {
        ArgumentNullException.ThrowIfNull(httpMessageHandlerFactory);
        ArgumentNullException.ThrowIfNull(reconnectStrategy);
        ArgumentNullException.ThrowIfNull(systemClock);
        ArgumentNullException.ThrowIfNull(logger);

        _httpMessageHandlerFactory = httpMessageHandlerFactory;
        _reconnectStrategy = reconnectStrategy;
        _systemClock = systemClock;
        _logger = logger;
    }

    public ILavalinkSocket? Create(IOptions<LavalinkSocketOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return new LavalinkSocket(_httpMessageHandlerFactory, _reconnectStrategy, _systemClock, _logger, options);
    }
}
