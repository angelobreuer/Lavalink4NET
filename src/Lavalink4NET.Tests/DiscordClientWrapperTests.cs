namespace Lavalink4NET.Tests;

using System;
using global::Discord.WebSocket;
using Lavalink4NET.DiscordNet;
using Xunit;

public sealed class DiscordClientWrapperTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void TestConstructorThrowsIfInvalidShardCountPassed(int shards)
    {
        var client = new DiscordShardedClient();
        Assert.Throws<ArgumentOutOfRangeException>(() => new DiscordClientWrapper(client, shards));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(20)]
    [InlineData(100)]
    public void TestConstructorDoesNotThrowIfValidShardCountPassed(int shards)
    {
        var client = new DiscordShardedClient();
        _ = new DiscordClientWrapper(client, shards);
    }
}
