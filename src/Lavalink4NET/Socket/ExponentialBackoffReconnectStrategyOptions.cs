namespace Lavalink4NET.Socket;

using System;

public sealed class ExponentialBackoffReconnectStrategyOptions
{
    public TimeSpan MaximumDelay { get; set; } = TimeSpan.FromMinutes(1);

    public TimeSpan MinimumDelay { get; set; } = TimeSpan.FromSeconds(2);

    public TimeSpan InitialDelay { get; set; } = TimeSpan.FromSeconds(2);

    public double BackoffMultiplier { get; set; } = 2.0;
}
