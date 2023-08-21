namespace Lavalink4NET.Tests.Players.Preconditions;

using System.Threading.Tasks;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Preconditions;
using Lavalink4NET.Players.Queued;
using Moq;
using Xunit;

public sealed class PlayerPreconditionTests
{
    [Fact]
    public async Task TestPlayingPreconditionAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayer>(x => x.State == PlayerState.Playing);

        // Act
        var result = await PlayerPrecondition.Playing
            .CheckAsync(player)
            .ConfigureAwait(false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TestPlayingPreconditionNonMatchingAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayer>(x => x.State == PlayerState.Paused);

        // Act
        var result = await PlayerPrecondition.Playing
            .CheckAsync(player)
            .ConfigureAwait(false);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TestNotPlayingPreconditionAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayer>(x => x.State == PlayerState.NotPlaying);

        // Act
        var result = await PlayerPrecondition.NotPlaying
            .CheckAsync(player)
            .ConfigureAwait(false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TestNotPlayingPreconditionNonMatchingAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayer>(x => x.State == PlayerState.Paused);

        // Act
        var result = await PlayerPrecondition.NotPlaying
            .CheckAsync(player)
            .ConfigureAwait(false);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TestPausedPreconditionAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayer>(x => x.State == PlayerState.Paused);

        // Act
        var result = await PlayerPrecondition.Paused
            .CheckAsync(player)
            .ConfigureAwait(false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TestPausedPreconditionNonMatchingAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayer>(x => x.State == PlayerState.NotPlaying);

        // Act
        var result = await PlayerPrecondition.Paused
            .CheckAsync(player)
            .ConfigureAwait(false);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TestNotPausedPreconditionAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayer>(x => x.State == PlayerState.NotPlaying);

        // Act
        var result = await PlayerPrecondition.NotPaused
            .CheckAsync(player)
            .ConfigureAwait(false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TestNotPausedPreconditionNonMatchingAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayer>(x => x.State == PlayerState.Paused);

        // Act
        var result = await PlayerPrecondition.NotPaused
            .CheckAsync(player)
            .ConfigureAwait(false);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TestQueueEmptyPreconditionAsync()
    {
        // Arrange
        var player = Mock.Of<IQueuedLavalinkPlayer>(x => x.Queue.IsEmpty);

        // Act
        var result = await PlayerPrecondition.QueueEmpty
            .CheckAsync(player)
            .ConfigureAwait(false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TestQueueEmptyPreconditionNonMatchingAsync()
    {
        // Arrange
        var player = Mock.Of<IQueuedLavalinkPlayer>(x => !x.Queue.IsEmpty);

        // Act
        var result = await PlayerPrecondition.QueueEmpty
            .CheckAsync(player)
            .ConfigureAwait(false);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TestQueueNotEmptyPreconditionAsync()
    {
        // Arrange
        var player = Mock.Of<IQueuedLavalinkPlayer>(x => !x.Queue.IsEmpty);

        // Act
        var result = await PlayerPrecondition.QueueNotEmpty
            .CheckAsync(player)
            .ConfigureAwait(false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TestQueueNotEmptyPreconditionNonMatchingAsync()
    {
        // Arrange
        var player = Mock.Of<IQueuedLavalinkPlayer>(x => x.Queue.IsEmpty);

        // Act
        var result = await PlayerPrecondition.QueueNotEmpty
            .CheckAsync(player)
            .ConfigureAwait(false);

        // Assert
        Assert.False(result);
    }
}
