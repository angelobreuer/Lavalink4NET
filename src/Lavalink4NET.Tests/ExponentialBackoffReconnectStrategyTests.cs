namespace Lavalink4NET.Tests;

using System;
using System.Threading.Tasks;
using Lavalink4NET.Socket;
using Microsoft.Extensions.Options;
using Xunit;

public sealed class ExponentialBackoffReconnectStrategyTests
{
    [Fact]
    public async Task DefaultReconnectStrategyTestAsync()
    {
        var strategy = new ExponentialBackoffReconnectStrategy(
            Options.Create(new ExponentialBackoffReconnectStrategyOptions()));

        Assert.Equal(TimeSpan.FromSeconds(2), await strategy.GetNextDelayAsync(DateTimeOffset.UtcNow, 1));
        Assert.Equal(TimeSpan.FromSeconds(4), await strategy.GetNextDelayAsync(DateTimeOffset.UtcNow, 2));
        Assert.Equal(TimeSpan.FromSeconds(8), await strategy.GetNextDelayAsync(DateTimeOffset.UtcNow, 3));
        Assert.Equal(TimeSpan.FromSeconds(16), await strategy.GetNextDelayAsync(DateTimeOffset.UtcNow, 4));
        Assert.Equal(TimeSpan.FromSeconds(60), await strategy.GetNextDelayAsync(DateTimeOffset.UtcNow, 50));
        Assert.Equal(TimeSpan.FromSeconds(60), await strategy.GetNextDelayAsync(DateTimeOffset.UtcNow, 50000));
    }
}
