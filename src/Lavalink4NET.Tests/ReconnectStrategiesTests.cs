namespace Lavalink4NET.Tests;

using System;
using Xunit;

/// <summary>
///     Contains test for the <see cref="ReconnectStrategies"/>.
/// </summary>
public sealed class ReconnectStrategiesTests
{
    /// <summary>
    ///     Test the <see cref="ReconnectStrategies.DefaultStrategy"/> reconnect strategy logic.
    /// </summary>
    [Fact]
    public void DefaultReconnectStrategyTest()
    {
        var strategy = ReconnectStrategies.DefaultStrategy;

        Assert.Equal(TimeSpan.FromSeconds(5 * 3), strategy(DateTimeOffset.UtcNow, 1));
        Assert.Equal(TimeSpan.FromSeconds(5 * 3), strategy(DateTimeOffset.UtcNow, 2));
        Assert.Equal(TimeSpan.FromSeconds(5 * 3), strategy(DateTimeOffset.UtcNow, 3));
        Assert.Equal(TimeSpan.FromSeconds(5 * 4), strategy(DateTimeOffset.UtcNow, 4));
        Assert.Equal(TimeSpan.FromSeconds(5 * 50), strategy(DateTimeOffset.UtcNow, 50));
        Assert.Equal(TimeSpan.FromSeconds(5 * 50000), strategy(DateTimeOffset.UtcNow, 50000));
    }

    /// <summary>
    ///     Test the <see cref="ReconnectStrategies.None"/> reconnect strategy logic.
    /// </summary>
    [Fact]
    public void NoneReconnectStrategyTest()
    {
        var strategy = ReconnectStrategies.None;

        Assert.Null(strategy(DateTimeOffset.UtcNow, 0));
        Assert.Null(strategy(DateTimeOffset.UtcNow, 3));
        Assert.Null(strategy(DateTimeOffset.UtcNow, 10));
        Assert.Null(strategy(DateTimeOffset.UtcNow, 10000));
    }
}
