namespace Lavalink4NET.Tests;

using System;
using System.Reflection;
using System.Threading.Tasks;
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
        // Arrange
        var client = new DiscordShardedClient();

        // Act
        void Act()
        {
            using var _ = new DiscordClientWrapper(client, shards);
        }

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(20)]
    [InlineData(100)]
    public async Task TestConstructorDoesNotThrowIfValidShardCountPassedAsync(int shards)
    {
        // Arrange
        await using var client = new DiscordShardedClient();

        // Act
        var exception = Record.Exception(() =>
        {
            using var _ = new DiscordClientWrapper(client, shards);
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task TestCreateUsingShardedClientWithoutShardsAsync()
    {
        // Arrange
        await using var client = new DiscordShardedClient();

        // Act
        var exception = Record.Exception(() =>
        {
            using var _ = new DiscordClientWrapper(client);
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task TestCreateUsingShardedClientWithExplicitShardsAsync()
    {
        // Arrange
        await using var client = new DiscordShardedClient();

        // Act
        var exception = Record.Exception(() =>
        {
            using var _ = new DiscordClientWrapper(client, 8);
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task TestCreateUsingNonShardedClientWithShardsAsync()
    {
        // Arrange
        await using var client = new DiscordSocketClient();

        // Act
        var exception = Record.Exception(() =>
        {
            using var _ = new DiscordClientWrapper(client, 8);
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task TestCreateUsingNonShardedClientWithNoShardsAsync()
    {
        // Arrange
        await using var client = new DiscordSocketClient();

        // Act
        var exception = Record.Exception(() =>
        {
            using var _ = new DiscordClientWrapper(client);
        });

        // Assert
        Assert.Null(exception);
    }
}

file static class ReflectionHelpers
{
    public static object CreateSocketGlobalUser(DiscordSocketClient discordSocketClient, ulong id)
    {
        var socketGlobalUserType = typeof(SocketUser).Assembly.GetType("Discord.WebSocket.SocketGlobalUser")!;

        var socketGlobalUserCtor = socketGlobalUserType.GetConstructor(
            bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
            types: new[] { typeof(DiscordSocketClient), typeof(ulong), })!;

        var parameters = new object[] { discordSocketClient, id, };
        var socketGlobalUser = socketGlobalUserCtor.Invoke(parameters);
        return socketGlobalUser;
    }

    public static SocketGuild CreateSocketGuild(DiscordSocketClient discordSocketClient, ulong id)
    {
        var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
        var socketGuildCtor = typeof(SocketGuild).GetConstructor(
         bindingAttr,
         null, new[]{
                    typeof(DiscordSocketClient),
                    typeof(ulong),
         }, null);

        var socketGuild = (SocketGuild)socketGuildCtor.Invoke(new object[] {
                discordSocketClient, id,
            });
        return socketGuild;
    }

    public static SocketGuildUser CreateSocketGuildUser(SocketGuild socketGuild, object socketGlobalUser)
    {
        var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
        var types = new[]{
                typeof(SocketGuild),
                socketGlobalUser.GetType(),
            };
        var socketGuildUserCtor = typeof(SocketGuildUser).GetConstructor(
           bindingAttr,
           null, types, null);

        var parameters = new object[] {
                socketGuild, socketGlobalUser
            };

        var socketGuildUser = (SocketGuildUser)socketGuildUserCtor.Invoke(parameters);

        return socketGuildUser;
    }
}