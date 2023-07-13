namespace Lavalink4NET;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players;
using Microsoft.Extensions.DependencyInjection;

internal sealed class LavalinkSessionProvider : ILavalinkSessionProvider
{
    private readonly IServiceProvider _serviceProvider;
    private AudioService? _audioService;

    public LavalinkSessionProvider(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    public async ValueTask<LavalinkPlayerSession> GetSessionAsync(ulong guildId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _audioService ??= (AudioService)_serviceProvider.GetRequiredService<IAudioService>();

        var apiClient = await _audioService.ApiClientProvider
            .GetClientAsync(cancellationToken)
            .ConfigureAwait(false);

        return new LavalinkPlayerSession(apiClient, _audioService.SessionId!);
    }
}
