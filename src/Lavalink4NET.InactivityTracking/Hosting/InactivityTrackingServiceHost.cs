namespace Lavalink4NET.InactivityTracking.Hosting;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

internal sealed class InactivityTrackingServiceHost : IHostedService
{
    private readonly IInactivityTrackingService _inactivityTrackingService;

    public InactivityTrackingServiceHost(IInactivityTrackingService inactivityTrackingService)
    {
        ArgumentNullException.ThrowIfNull(inactivityTrackingService);

        _inactivityTrackingService = inactivityTrackingService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _inactivityTrackingService.StartAsync(cancellationToken).AsTask();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _inactivityTrackingService.StopAsync(cancellationToken).AsTask();
    }
}
