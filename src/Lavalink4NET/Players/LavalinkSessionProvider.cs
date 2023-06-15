namespace Lavalink4NET.Players;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

internal sealed class LavalinkSessionProvider : ILavalinkSessionProvider
{
    private readonly IServiceProvider _serviceProvider;

    public LavalinkSessionProvider(IServiceProvider serviceProvider)
    {
        // Use service provider to avoid circular dependency
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    public async ValueTask<string> GetSessionIdAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var audioService = _serviceProvider.GetRequiredService<IAudioService>();

        await audioService
            .WaitForReadyAsync(cancellationToken)
            .ConfigureAwait(false);

        var sessionId = audioService.SessionId;
        Debug.Assert(sessionId is not null);
        return sessionId;
    }
}
