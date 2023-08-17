namespace Lavalink4NET.InactivityTracking.Tests;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.InactivityTracking.Players;
using Lavalink4NET.InactivityTracking.Trackers;
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
            discordClient: Mock.Of<IDiscordClientWrapper>(),
            new SystemClock(),
            Options.Create(new InactivityTrackingOptions()),
            logger: NullLogger<InactivityTrackingService>.Instance);

        // Act
        await inactivityTrackingService.StartAsync();

        // Assert
        Assert.Equal(InactivityTrackingState.Running, inactivityTrackingService.State);
    }

    [Fact]
    public async Task TestSubsequentStartReturnsCompletedTaskAsync()
    {
        // Arrange
        var playerManager = Mock.Of<IPlayerManager>();

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            discordClient: Mock.Of<IDiscordClientWrapper>(),
            new SystemClock(),
            Options.Create(new InactivityTrackingOptions()),
            logger: NullLogger<InactivityTrackingService>.Instance);

        await inactivityTrackingService.StartAsync();

        // Act
        var task = inactivityTrackingService.StartAsync();

        // Assert
        Assert.True(task.IsCompleted);

        // Clean Up
        await task.ConfigureAwait(false);
    }

    [Fact]
    public async Task TestStateIsStoppedAfterStopAsync()
    {
        // Arrange
        var playerManager = Mock.Of<IPlayerManager>();

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            discordClient: Mock.Of<IDiscordClientWrapper>(),
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

#pragma warning disable S3966 // Objects should not be disposed more than once
        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            discordClient: Mock.Of<IDiscordClientWrapper>(),
            systemClock: new SystemClock(),
            options: Options.Create(new InactivityTrackingOptions()),
            logger: NullLogger<InactivityTrackingService>.Instance);

        // Act
        await inactivityTrackingService.StartAsync();
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
            discordClient: Mock.Of<IDiscordClientWrapper>(),
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
    public async Task TestPlayerIsNotTrackedInitially()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayerWithListener>();
        var playerManager = Mock.Of<IPlayerManager>(x => x.Players == new[] { player as ILavalinkPlayer, });
        var inactivityTracker = new DynamicTracker();
        var discordClient = Mock.Of<IDiscordClientWrapper>();

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            discordClient: discordClient,
            systemClock: new SystemClock(),
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create<IInactivityTracker>(inactivityTracker), }),
            logger: NullLogger<InactivityTrackingService>.Instance);

        // Act
        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        // Assert
        var trackingInformation = await inactivityTrackingService
            .GetPlayerAsync(player)
            .ConfigureAwait(false);

        Assert.Equal(PlayerTrackingStatus.NotTracked, trackingInformation.Status);
    }

    [Fact]
    public async Task TestPlayerIsTrackedWhenInactiveWithoutDeadlineAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayerWithListener>();
        var playerManager = Mock.Of<IPlayerManager>(x => x.Players == new[] { player as ILavalinkPlayer, });
        var inactivityTracker = new DynamicTracker();
        var discordClient = Mock.Of<IDiscordClientWrapper>();

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            discordClient: discordClient,
            systemClock: new SystemClock(),
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create<IInactivityTracker>(inactivityTracker), }),
            logger: NullLogger<InactivityTrackingService>.Instance);

        inactivityTracker.Result = PlayerActivityResult.Inactive(timeout: TimeSpan.FromHours(1));

        // Act
        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        // Assert
        var trackingInformation = await inactivityTrackingService
            .GetPlayerAsync(player)
            .ConfigureAwait(false);

        Assert.Equal(PlayerTrackingStatus.Tracked, trackingInformation.Status);
    }

    [Fact]
    public async Task TestPlayerIsInactiveWhenInactiveAfterDeadlineAsync()
    {
        // Arrange
        var player = new Mock<ILavalinkPlayerWithListener>();
        var playerManager = Mock.Of<IPlayerManager>(x => x.Players == new[] { player.Object as ILavalinkPlayer, });
        var inactivityTracker = new DynamicTracker();
        var systemClock = new SystemClock();
        var discordClient = Mock.Of<IDiscordClientWrapper>();

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            discordClient: discordClient,
            systemClock: systemClock,
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create<IInactivityTracker>(inactivityTracker), }),
            logger: NullLogger<InactivityTrackingService>.Instance);

        inactivityTracker.Result = PlayerActivityResult.Inactive();

        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        systemClock.UtcNow = systemClock.UtcNow.AddDays(1);

        // Act
        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        // Assert
        player.Verify(x => x.NotifyPlayerInactiveAsync(It.IsAny<PlayerTrackingState>(), It.IsAny<PlayerTrackingState>(), It.IsAny<IInactivityTracker>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task TestPlayerIsInactiveWhenInactiveAfterDeadlineWithInitialActivePeriodAsync()
    {
        // Arrange
        var player = new Mock<ILavalinkPlayerWithListener>();
        var playerManager = Mock.Of<IPlayerManager>(x => x.Players == new[] { player.Object as ILavalinkPlayer, });
        var inactivityTracker = new DynamicTracker();
        var systemClock = new SystemClock();
        var discordClient = Mock.Of<IDiscordClientWrapper>();

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            discordClient: discordClient,
            systemClock: systemClock,
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create<IInactivityTracker>(inactivityTracker), }),
            logger: NullLogger<InactivityTrackingService>.Instance);

        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        systemClock.UtcNow = systemClock.UtcNow.AddDays(1);
        inactivityTracker.Result = PlayerActivityResult.Inactive();

        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        systemClock.UtcNow = systemClock.UtcNow.AddDays(1);

        // Act
        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        // Assert
        player.Verify(x => x.NotifyPlayerInactiveAsync(It.IsAny<PlayerTrackingState>(), It.IsAny<PlayerTrackingState>(), It.IsAny<IInactivityTracker>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task TestPlayerRemovedFromTrackingListIfRemovedFromPlayerManagerAsync()
    {
        // Arrange
        var player1 = Mock.Of<ILavalinkPlayerWithListener>(x => x.GuildId == 1UL);
        var player2 = Mock.Of<ILavalinkPlayerWithListener>(x => x.GuildId == 2UL);
        var players = new[] { player1 as ILavalinkPlayer, };
        var playerManager = Mock.Of<IPlayerManager>(x => x.Players == players);
        var inactivityTracker = new DynamicTracker();
        var systemClock = new SystemClock();
        var discordClient = Mock.Of<IDiscordClientWrapper>();

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            discordClient: discordClient,
            systemClock: systemClock,
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create<IInactivityTracker>(inactivityTracker), }),
            logger: NullLogger<InactivityTrackingService>.Instance);

        inactivityTracker.Result = PlayerActivityResult.Inactive();

        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        players[0] = player2;

        // Act
        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        // Assert
        var trackingInformation = await inactivityTrackingService
            .GetPlayerAsync(player1)
            .ConfigureAwait(false);

        Assert.Equal(PlayerTrackingStatus.NotTracked, trackingInformation.Status);
    }

    [Fact]
    public async Task TestPlayerTrackedButThenActiveAgainAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayerWithListener>();
        var playerManager = Mock.Of<IPlayerManager>(x => x.Players == new[] { player as ILavalinkPlayer, });
        var inactivityTracker = new DynamicTracker();
        var systemClock = new SystemClock();
        var discordClient = Mock.Of<IDiscordClientWrapper>();

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            discordClient: discordClient,
            systemClock: systemClock,
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create<IInactivityTracker>(inactivityTracker), }),
            logger: NullLogger<InactivityTrackingService>.Instance);

        inactivityTracker.Result = PlayerActivityResult.Inactive();

        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        systemClock.UtcNow = systemClock.UtcNow.AddDays(1);

        inactivityTracker.Result = PlayerActivityResult.Active;

        // Act
        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        // Assert
        var trackingInformation = await inactivityTrackingService
            .GetPlayerAsync(player)
            .ConfigureAwait(false);

        Assert.Equal(PlayerTrackingStatus.NotTracked, trackingInformation.Status);
    }

    [Fact]
    public async Task TestPlayerInactiveAsync()
    {
        // Arrange
        var player = new Mock<ILavalinkPlayerWithListener>();

        var playerManager = Mock.Of<IPlayerManager>(x => x.Players == new[] { player.Object as ILavalinkPlayer, });
        var inactivityTracker = new DynamicTracker();
        var systemClock = new SystemClock();
        var discordClient = Mock.Of<IDiscordClientWrapper>();

        using var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            discordClient: discordClient,
            systemClock: systemClock,
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create<IInactivityTracker>(inactivityTracker), }),
            logger: NullLogger<InactivityTrackingService>.Instance);

        inactivityTracker.Result = PlayerActivityResult.Inactive();

        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        systemClock.UtcNow = systemClock.UtcNow.AddDays(1);

        // Act
        await inactivityTrackingService
            .PollAsync()
            .ConfigureAwait(false);

        // Assert
        player.Verify(x => x.NotifyPlayerInactiveAsync(It.IsAny<PlayerTrackingState>(), It.IsAny<PlayerTrackingState>(), It.IsAny<IInactivityTracker>(), It.IsAny<CancellationToken>()));
    }
}

public interface ILavalinkPlayerWithListener : ILavalinkPlayer, IInactivityPlayerListener
{
}

file sealed class SystemClock : ISystemClock
{
    public DateTimeOffset UtcNow { get; set; } = DateTimeOffset.UtcNow;
}

file sealed class DynamicTracker : IInactivityTracker
{
    public PlayerActivityResult Result { get; set; } = PlayerActivityResult.Active;

    public ValueTask<PlayerActivityResult> CheckAsync(InactivityTrackingContext context, ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        return new ValueTask<PlayerActivityResult>(Result);
    }
}