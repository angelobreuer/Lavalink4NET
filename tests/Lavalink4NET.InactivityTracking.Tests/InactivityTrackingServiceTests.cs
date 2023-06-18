namespace Lavalink4NET.InactivityTracking.Tests;

using System.Collections.Immutable;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.InactivityTracking.Events;
using Lavalink4NET.Players;
using Lavalink4NET.Tracking;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

public sealed class InactivityTrackingServiceTests
{
    [Fact]
    public async Task TestStateIsRunningAfterStartAsync()
    {
        // Arrange
        var playerManager = Mock.Of<IPlayerManager>();

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            clientWrapper: Mock.Of<IDiscordClientWrapper>(),
            new SystemClock(),
            Options.Create(new InactivityTrackingOptions()),
            logger: NullLogger<InactivityTrackingService>.Instance);

        // Act
        await inactivityTrackingService.StartAsync();

        // Assert
        Assert.Equal(InactivityTrackingState.Running, inactivityTrackingService.State);
    }

    [Fact]
    public async Task TestStateIsStoppedAfterStopAsync()
    {
        // Arrange
        var playerManager = Mock.Of<IPlayerManager>();

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            clientWrapper: Mock.Of<IDiscordClientWrapper>(),
            systemClock: new SystemClock(),
            options: Options.Create(new InactivityTrackingOptions()),
            logger: NullLogger<InactivityTrackingService>.Instance);

        // Act
        await inactivityTrackingService.StartAsync();
        await inactivityTrackingService.StopAsync();

        // Assert
        Assert.Equal(InactivityTrackingState.Stopped, inactivityTrackingService.State);
    }

    [Fact]
    public async Task TestStateIsDestroyedAfterDispose()
    {
        // Arrange
        var playerManager = Mock.Of<IPlayerManager>();

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            clientWrapper: Mock.Of<IDiscordClientWrapper>(),
            systemClock: new SystemClock(),
            options: Options.Create(new InactivityTrackingOptions()),
            logger: NullLogger<InactivityTrackingService>.Instance);

        // Act
        await inactivityTrackingService.StartAsync();
#pragma warning disable S3966 // Objects should not be disposed more than once
        inactivityTrackingService.Dispose();
#pragma warning restore S3966 // Objects should not be disposed more than once

        // Assert
        Assert.Equal(InactivityTrackingState.Destroyed, inactivityTrackingService.State);
    }

    [Fact]
    public async Task TestStartAfterStopThrowsAsync()
    {
        // Arrange
        var playerManager = Mock.Of<IPlayerManager>();

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            clientWrapper: Mock.Of<IDiscordClientWrapper>(),
            systemClock: new SystemClock(),
            options: Options.Create(new InactivityTrackingOptions()),
            logger: NullLogger<InactivityTrackingService>.Instance);

        // Act
        await inactivityTrackingService.StartAsync();
        await inactivityTrackingService.StopAsync();

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => inactivityTrackingService.StartAsync().AsTask());
    }

    [Fact]
    public async Task TestPlayerInitiallyNotTrackedAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayer>();
        var playerManager = Mock.Of<IPlayerManager>(x => x.Players == new[] { player, });

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            clientWrapper: Mock.Of<IDiscordClientWrapper>(),
            systemClock: new SystemClock(),
            options: Options.Create(new InactivityTrackingOptions()),
            logger: NullLogger<InactivityTrackingService>.Instance);

        // Act
        await inactivityTrackingService.StartAsync();

        // Assert
        Assert.Equal(InactivityTrackingStatus.NotTracked, inactivityTrackingService.GetStatus(Mock.Of<ILavalinkPlayer>()));
    }

    [Fact]
    public async Task TestPlayerTrackedIfInactiveAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayer>();
        var playerManager = Mock.Of<IPlayerManager>(x => x.Players == new[] { player, });

        var tracker = Mock.Of<IInactivityTracker>(x
            => x.CheckAsync(It.IsAny<InactivityTrackingContext>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(true));

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            clientWrapper: Mock.Of<IDiscordClientWrapper>(),
            systemClock: new SystemClock(),
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create(tracker), }),
            logger: NullLogger<InactivityTrackingService>.Instance);

        // Act
        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        // Assert
        Assert.Equal(InactivityTrackingStatus.Tracked, inactivityTrackingService.GetStatus(player));
    }

    [Fact]
    public async Task TestPlayerMarkedInactiveAfterDelayAsync()
    {
        // Arrange
        var currentTime = DateTimeOffset.UtcNow;

        var systemClock = new Mock<ISystemClock>();
        systemClock.SetupGet(x => x.UtcNow).Returns(() => currentTime);

        var player = Mock.Of<ILavalinkPlayer>();
        var playerManager = Mock.Of<IPlayerManager>(x => x.Players == new[] { player, });

        var tracker = Mock.Of<IInactivityTracker>(x
            => x.CheckAsync(It.IsAny<InactivityTrackingContext>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(true));

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            clientWrapper: Mock.Of<IDiscordClientWrapper>(),
            systemClock: systemClock.Object,
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create(tracker), }),
            logger: NullLogger<InactivityTrackingService>.Instance);

        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        currentTime = currentTime.AddDays(1);

        // Act
        var status = inactivityTrackingService.GetStatus(player);

        // Assert
        Assert.Equal(InactivityTrackingStatus.Inactive, status);
    }

    [Fact]
    public async Task TestPlayerDestroyedAfterInactivityTimeoutAsync()
    {
        // Arrange
        var currentTime = DateTimeOffset.UtcNow;

        var systemClock = new Mock<ISystemClock>();
        systemClock.SetupGet(x => x.UtcNow).Returns(() => currentTime);

        var player = new Mock<ILavalinkPlayer>();
        player.Setup(x => x.DisposeAsync()).Returns(ValueTask.CompletedTask).Verifiable();

        var playerManager = Mock.Of<IPlayerManager>(x => x.Players == new[] { player.Object, });

        var tracker = Mock.Of<IInactivityTracker>(x
            => x.CheckAsync(It.IsAny<InactivityTrackingContext>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(true));

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            clientWrapper: Mock.Of<IDiscordClientWrapper>(),
            systemClock: systemClock.Object,
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create(tracker), }),
            logger: NullLogger<InactivityTrackingService>.Instance);

        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        currentTime = currentTime.AddDays(1);

        // Act
        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        // Assert
        player.Verify();
    }

    [Fact]
    public async Task TestPlayerNotDestroyedAfterInactivityTimeoutIfEventDiscardsAsync()
    {
        // Arrange
        var currentTime = DateTimeOffset.UtcNow;

        var systemClock = new Mock<ISystemClock>();
        systemClock.SetupGet(x => x.UtcNow).Returns(() => currentTime);

        var player = new Mock<ILavalinkPlayer>();
        player.Setup(x => x.DisposeAsync()).Returns(ValueTask.CompletedTask).Verifiable();

        var playerManager = Mock.Of<IPlayerManager>(x => x.Players == new[] { player.Object, });

        var tracker = Mock.Of<IInactivityTracker>(x
            => x.CheckAsync(It.IsAny<InactivityTrackingContext>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(true));

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            clientWrapper: Mock.Of<IDiscordClientWrapper>(),
            systemClock: systemClock.Object,
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create(tracker), }),
            logger: NullLogger<InactivityTrackingService>.Instance);

        static Task Handler(object? sender, InactivePlayerEventArgs eventArgs)
        {
            eventArgs.ShouldStop = false;
            return Task.CompletedTask;
        }

        inactivityTrackingService.InactivePlayer += Handler;

        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        currentTime = currentTime.AddDays(1);

        // Act
        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        // Assert
        player.Verify(x => x.DisposeAsync(), Times.Never());

        // Cleanup
        inactivityTrackingService.InactivePlayer -= Handler;
    }

    [Fact]
    public async Task TestPlayerRemovedFromTrackingListIfActiveAgainAsync()
    {
        // Arrange
        var inactive = true;

        var player = new Mock<ILavalinkPlayer>();
        player.Setup(x => x.DisposeAsync()).Returns(ValueTask.CompletedTask).Verifiable();

        var playerManager = Mock.Of<IPlayerManager>(x => x.Players == new[] { player.Object, });

        var tracker = new Mock<IInactivityTracker>();

        tracker
            .Setup(x => x.CheckAsync(It.IsAny<InactivityTrackingContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => inactive);

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            clientWrapper: Mock.Of<IDiscordClientWrapper>(),
            systemClock: new SystemClock(),
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create(tracker.Object), }),
            logger: NullLogger<InactivityTrackingService>.Instance);

        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        inactive = false;

        // Act
        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        // Assert
        Assert.Equal(InactivityTrackingStatus.NotTracked, inactivityTrackingService.GetStatus(player.Object));
    }

    [Fact]
    public async Task TestDestroyedPlayerIsRemovedFromTrackingListAsync()
    {
        // Arrange
        var player = new Mock<ILavalinkPlayer>();
        var players = ImmutableArray.Create(player.Object);

        player.Setup(x => x.DisposeAsync()).Returns(ValueTask.CompletedTask).Verifiable();

        var playerManager = new Mock<IPlayerManager>();
        playerManager.SetupGet(x => x.Players).Returns(() => players);

        var tracker = new Mock<IInactivityTracker>();

        tracker
            .Setup(x => x.CheckAsync(It.IsAny<InactivityTrackingContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => true);

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager.Object,
            clientWrapper: Mock.Of<IDiscordClientWrapper>(),
            systemClock: new SystemClock(),
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create(tracker.Object), }),
            logger: NullLogger<InactivityTrackingService>.Instance);

        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        players = ImmutableArray<ILavalinkPlayer>.Empty;

        // Act
        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        // Assert
        Assert.Equal(InactivityTrackingStatus.NotTracked, inactivityTrackingService.GetStatus(player.Object));
    }
}
