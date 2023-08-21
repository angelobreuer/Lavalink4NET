namespace Lavalink4NET.InactivityTracking.Hosting;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.InactivityTracking.Queue;
using Microsoft.Extensions.Hosting;

internal sealed class InactivityExpirationQueueHost : BackgroundService
{
    private readonly IInactivityExpirationQueue _inactivityExpirationQueue;

    public InactivityExpirationQueueHost(IInactivityExpirationQueue inactivityExpirationQueue)
    {
        ArgumentNullException.ThrowIfNull(inactivityExpirationQueue);

        _inactivityExpirationQueue = inactivityExpirationQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        while (!stoppingToken.IsCancellationRequested)
        {
            var player = await _inactivityExpirationQueue
                .GetExpiredPlayerAsync(stoppingToken)
                .ConfigureAwait(false);

            if (player is null)
            {
                break;
            }

            await using var _ = player.ConfigureAwait(false);
        }
    }
}
