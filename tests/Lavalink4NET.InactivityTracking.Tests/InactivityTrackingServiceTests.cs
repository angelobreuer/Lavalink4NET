namespace Lavalink4NET.InactivityTracking.Tests;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.InactivityTracking.Players;
using Lavalink4NET.InactivityTracking.Queue;
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
    public async Task TestPlayerIsNotTrackedInitially()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayerWithListener>();
        var playerManager = Mock.Of<IPlayerManager>(x => x.Players == new[] { player as ILavalinkPlayer, });
        var inactivityTracker = new DynamicTracker(player);
        var discordClient = Mock.Of<IDiscordClientWrapper>();

        var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            expirationQueue: Mock.Of<IInactivityExpirationQueue>(),
            discordClient: discordClient,
            systemClock: new SystemClock(),
            trackers: Enumerable.Empty<IInactivityTracker>(),
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create<IInactivityTracker>(inactivityTracker), }),
            loggerFactory: NullLoggerFactory.Instance);

        await using var __ = inactivityTrackingService.ConfigureAwait(false);

        // Act
        var trackingInformation = await inactivityTrackingService
            .GetPlayerAsync(player)
            .ConfigureAwait(false);

        // Assert
        Assert.Equal(PlayerTrackingStatus.NotTracked, trackingInformation.Status);
    }

    [Fact]
    public async Task TestPlayerIsTrackedWhenInactiveAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayerWithListener>();

        var discordClient = Mock.Of<IDiscordClientWrapper>();
        var playerManager = new PlayerManagerMock(discordClient, new[] { player });
        var inactivityTracker = new DynamicTracker(player);

        var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            expirationQueue: Mock.Of<IInactivityExpirationQueue>(),
            discordClient: discordClient,
            systemClock: new SystemClock(),
            trackers: Enumerable.Empty<IInactivityTracker>(),
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create<IInactivityTracker>(inactivityTracker), }),
            loggerFactory: NullLoggerFactory.Instance);

        await using var _ = inactivityTrackingService.ConfigureAwait(false);

        await inactivityTrackingService
            .StartAsync()
            .ConfigureAwait(false);

        inactivityTracker.Report(active: false);

        // Assert
        var trackingInformation = await inactivityTrackingService
            .GetPlayerAsync(player)
            .ConfigureAwait(false);

        Assert.Equal(PlayerTrackingStatus.Tracked, trackingInformation.Status);
    }

    [Fact]
    public async Task TestPlayerIsNotTrackedWhenActiveAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayerWithListener>();

        var discordClient = Mock.Of<IDiscordClientWrapper>();
        var playerManager = new PlayerManagerMock(discordClient, new[] { player });
        var inactivityTracker = new DynamicTracker(player);

        var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            expirationQueue: Mock.Of<IInactivityExpirationQueue>(),
            discordClient: discordClient,
            systemClock: new SystemClock(),
            trackers: Enumerable.Empty<IInactivityTracker>(),
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create<IInactivityTracker>(inactivityTracker), }),
            loggerFactory: NullLoggerFactory.Instance);

        await using var _ = inactivityTrackingService.ConfigureAwait(false);

        await inactivityTrackingService
            .StartAsync()
            .ConfigureAwait(false);

        inactivityTracker.Report(active: true);

        // Assert
        var trackingInformation = await inactivityTrackingService
            .GetPlayerAsync(player)
            .ConfigureAwait(false);

        Assert.Equal(PlayerTrackingStatus.NotTracked, trackingInformation.Status);
    }

    [Fact]
    public async Task TestPlayerIsNotTrackedWhenInactiveButThenActiveAgainAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayerWithListener>();

        var discordClient = Mock.Of<IDiscordClientWrapper>();
        var playerManager = new PlayerManagerMock(discordClient, new[] { player });
        var inactivityTracker = new DynamicTracker(player);

        var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            expirationQueue: Mock.Of<IInactivityExpirationQueue>(),
            discordClient: discordClient,
            systemClock: new SystemClock(),
            trackers: Enumerable.Empty<IInactivityTracker>(),
            options: Options.Create(new InactivityTrackingOptions { Trackers = ImmutableArray.Create<IInactivityTracker>(inactivityTracker), }),
            loggerFactory: NullLoggerFactory.Instance);

        await using var _ = inactivityTrackingService.ConfigureAwait(false);

        await inactivityTrackingService
            .StartAsync()
            .ConfigureAwait(false);

        inactivityTracker.Report(active: false);
        inactivityTracker.Report(active: true);

        // Assert
        var trackingInformation = await inactivityTrackingService
            .GetPlayerAsync(player)
            .ConfigureAwait(false);

        Assert.Equal(PlayerTrackingStatus.NotTracked, trackingInformation.Status);
    }

    [Fact]
    public async Task TestPlayerTimeoutIsLowestOfAllTrackersIfTimeoutBehaviorIsSetToLowestAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayerWithListener>();

        var relativeTime = DateTimeOffset.UtcNow;
        var systemClock = Mock.Of<ISystemClock>(x => x.UtcNow == relativeTime);
        var discordClient = Mock.Of<IDiscordClientWrapper>();
        var playerManager = new PlayerManagerMock(discordClient, new[] { player });

        var inactivityTracker1 = new DynamicTracker(player, TimeSpan.FromSeconds(60));
        var inactivityTracker2 = new DynamicTracker(player, TimeSpan.FromSeconds(30));

        var expirationQueue = new Mock<IInactivityExpirationQueue>();

        var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            expirationQueue: expirationQueue.Object,
            discordClient: discordClient,
            systemClock: systemClock,
            trackers: Enumerable.Empty<IInactivityTracker>(),
            options: Options.Create(new InactivityTrackingOptions
            {
                Trackers = ImmutableArray.Create<IInactivityTracker>(inactivityTracker1, inactivityTracker2),
                TimeoutBehavior = InactivityTrackingTimeoutBehavior.Lowest, // explicit
            }),
            loggerFactory: NullLoggerFactory.Instance);

        await using var _ = inactivityTrackingService.ConfigureAwait(false);

        await inactivityTrackingService
            .StartAsync()
            .ConfigureAwait(false);

        inactivityTracker1.Report(active: false);
        inactivityTracker2.Report(active: false);

        // Assert
        expirationQueue.Verify(x => x.TryNotify(player, relativeTime.AddSeconds(30)));
    }

    [Fact]
    public async Task TestPlayerTimeoutIsHighestOfAllTrackersIfTimeoutBehaviorIsSetToHighestAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayerWithListener>();

        var relativeTime = DateTimeOffset.UtcNow;
        var systemClock = Mock.Of<ISystemClock>(x => x.UtcNow == relativeTime);
        var discordClient = Mock.Of<IDiscordClientWrapper>();
        var playerManager = new PlayerManagerMock(discordClient, new[] { player });

        var inactivityTracker1 = new DynamicTracker(player, TimeSpan.FromSeconds(60));
        var inactivityTracker2 = new DynamicTracker(player, TimeSpan.FromSeconds(30));

        var expirationQueue = new Mock<IInactivityExpirationQueue>();

        var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            expirationQueue: expirationQueue.Object,
            discordClient: discordClient,
            systemClock: systemClock,
            trackers: Enumerable.Empty<IInactivityTracker>(),
            options: Options.Create(new InactivityTrackingOptions
            {
                Trackers = ImmutableArray.Create<IInactivityTracker>(inactivityTracker1, inactivityTracker2),
                TimeoutBehavior = InactivityTrackingTimeoutBehavior.Highest,
            }),
            loggerFactory: NullLoggerFactory.Instance);

        await using var _ = inactivityTrackingService.ConfigureAwait(false);

        await inactivityTrackingService
            .StartAsync()
            .ConfigureAwait(false);

        inactivityTracker1.Report(active: false);
        inactivityTracker2.Report(active: false);

        // Assert
        expirationQueue.Verify(x => x.TryNotify(player, relativeTime.AddSeconds(60)));
    }

    [Fact]
    public async Task TestPlayerTimeoutIsAverageOfAllTrackersIfTimeoutBehaviorIsSetToAverageAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayerWithListener>();

        var relativeTime = DateTimeOffset.UtcNow;
        var systemClock = Mock.Of<ISystemClock>(x => x.UtcNow == relativeTime);
        var discordClient = Mock.Of<IDiscordClientWrapper>();
        var playerManager = new PlayerManagerMock(discordClient, new[] { player });

        var inactivityTracker1 = new DynamicTracker(player, TimeSpan.FromSeconds(60));
        var inactivityTracker2 = new DynamicTracker(player, TimeSpan.FromSeconds(30));

        var expirationQueue = new Mock<IInactivityExpirationQueue>();

        var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            expirationQueue: expirationQueue.Object,
            discordClient: discordClient,
            systemClock: systemClock,
            trackers: Enumerable.Empty<IInactivityTracker>(),
            options: Options.Create(new InactivityTrackingOptions
            {
                Trackers = ImmutableArray.Create<IInactivityTracker>(inactivityTracker1, inactivityTracker2),
                TimeoutBehavior = InactivityTrackingTimeoutBehavior.Average,
            }),
            loggerFactory: NullLoggerFactory.Instance);

        await using var _ = inactivityTrackingService.ConfigureAwait(false);

        await inactivityTrackingService
            .StartAsync()
            .ConfigureAwait(false);

        inactivityTracker1.Report(active: false);
        inactivityTracker2.Report(active: false);

        // Assert
        expirationQueue.Verify(x => x.TryNotify(player, relativeTime.AddSeconds(45)));
    }

    [Fact]
    public async Task TestTrackerUpdateKeepsPreviousTimestampIfUpdatedAsync()
    {
        // Arrange
        var player = Mock.Of<ILavalinkPlayerWithListener>();
        var systemClock = new SystemClock();
        var discordClient = Mock.Of<IDiscordClientWrapper>();
        var playerManager = new PlayerManagerMock(discordClient, new[] { player });
        var inactivityTracker = new DynamicTracker(player);

        var inactivityTrackingService = new InactivityTrackingService(
            playerManager: playerManager,
            expirationQueue: Mock.Of<IInactivityExpirationQueue>(),
            discordClient: discordClient,
            systemClock: systemClock,
            trackers: Enumerable.Empty<IInactivityTracker>(),
            options: Options.Create(new InactivityTrackingOptions
            {
                Trackers = ImmutableArray.Create<IInactivityTracker>(inactivityTracker),
                DefaultTimeout = TimeSpan.FromSeconds(1000),
            }),
            loggerFactory: NullLoggerFactory.Instance);

        await using var _ = inactivityTrackingService.ConfigureAwait(false);

        await inactivityTrackingService
            .StartAsync()
            .ConfigureAwait(false);

        var initialTimestamp = systemClock.UtcNow;

        inactivityTracker.Report(active: false);

        systemClock.UtcNow = systemClock.UtcNow.AddSeconds(60);

        inactivityTracker.Report(active: false);

        // Assert
        var trackingInformation = await inactivityTrackingService
            .GetPlayerAsync(player)
            .ConfigureAwait(false);

        Assert.Equal(PlayerTrackingStatus.Tracked, trackingInformation.Status);

        Assert.Equal(
            expected: initialTimestamp.DateTime,
            actual: trackingInformation.Trackers[0].TrackedSince!.Value.DateTime,
            precision: TimeSpan.FromSeconds(1));
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
    private readonly ILavalinkPlayer _player;
    private readonly TimeSpan? _timeout;
    private readonly Channel<bool> _channel;
    private readonly SemaphoreSlim _semaphoreSlim;

    public DynamicTracker(ILavalinkPlayer player, TimeSpan? timeout = null)
    {
        ArgumentNullException.ThrowIfNull(player);

        _player = player;
        _timeout = timeout;
        _semaphoreSlim = new SemaphoreSlim(0, int.MaxValue);
        _channel = Channel.CreateUnbounded<bool>();
    }

    public InactivityTrackerOptions Options => InactivityTrackerOptions.Realtime("Dynamic Tracker");

    public void Report(bool active)
    {
        _channel.Writer.TryWrite(active);
        _semaphoreSlim.Wait();
    }

    public async ValueTask RunAsync(IInactivityTrackerContext trackerContext, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(trackerContext);

        await foreach (var isActive in _channel.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
        {
            using (var scope = trackerContext.CreateScope())
            {
                if (isActive)
                {
                    scope.MarkActive(_player.GuildId);
                }
                else
                {
                    scope.MarkInactive(_player.GuildId, _timeout);
                }
            }

            _semaphoreSlim.Release();
        }
    }
}