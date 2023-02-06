namespace Lavalink4NET.Socket;

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class LavalinkSocketFactory : ILavalinkSocketFactory
{
    private readonly ILogger<LavalinkSocket> _logger;

    public LavalinkSocketFactory(ILogger<LavalinkSocket> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    public ILavalinkSocket Create(IOptions<LavalinkSocketOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return new LavalinkSocket(_logger, options);
    }
}
