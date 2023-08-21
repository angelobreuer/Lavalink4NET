namespace Lavalink4NET;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

internal sealed class AudioServiceHost : IHostedService
{
    private readonly IAudioService _audioService;

    public AudioServiceHost(IAudioService audioService)
    {
        ArgumentNullException.ThrowIfNull(audioService);

        _audioService = audioService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _audioService.StartAsync(cancellationToken).AsTask();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _audioService.StopAsync(cancellationToken).AsTask();
    }
}
