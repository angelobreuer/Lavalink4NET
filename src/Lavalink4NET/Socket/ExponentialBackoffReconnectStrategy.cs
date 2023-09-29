namespace Lavalink4NET.Socket;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

public sealed class ExponentialBackoffReconnectStrategy : IReconnectStrategy
{
    private readonly ExponentialBackoffReconnectStrategyOptions _options;

    public ExponentialBackoffReconnectStrategy(IOptions<ExponentialBackoffReconnectStrategyOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Value;
    }

    public ValueTask<TimeSpan?> GetNextDelayAsync(DateTimeOffset interruptedAt, int attempt, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (attempt is < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(attempt));
        }

        var factor = Math.Pow(_options.BackoffMultiplier, attempt - 1);
        var delayTicks = _options.InitialDelay.TotalSeconds * factor;
        var backoff = TimeSpan.FromSeconds(Math.Clamp(delayTicks, _options.MinimumDelay.TotalSeconds, _options.MaximumDelay.TotalSeconds));

        return new ValueTask<TimeSpan?>(backoff);
    }
}
