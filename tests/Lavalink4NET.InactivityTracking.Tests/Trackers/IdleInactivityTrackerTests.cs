namespace Lavalink4NET.InactivityTracking.Tests.Trackers;
public sealed class IdleInactivityTrackerTests
{/*
    [Fact]
    public async Task CheckWhenPlayerPlayingNotIdleAsync()
    {
        // Arrange
        var inactivityTrackingService = Mock.Of<IInactivityTrackingService>();
        var discordClientWrapper = Mock.Of<IDiscordClientWrapper>();
        var player = Mock.Of<ILavalinkPlayer>(x => x.State == PlayerState.Playing);

        var context = new InactivityTrackingContext(
            InactivityTrackingService: inactivityTrackingService,
            Client: discordClientWrapper,
            Player: player);

        var tracker = new IdleInactivityTracker();

        // Act
        var result = await tracker
            .CheckAsync(context)
            .ConfigureAwait(false);

        // Assert
        Assert.Equal(PlayerActivityStatus.Active, result);
    }

    [Fact]
    public async Task CheckWhenPlayerNotPlayingIdleAsync()
    {
        // Arrange
        var inactivityTrackingService = Mock.Of<IInactivityTrackingService>();
        var discordClientWrapper = Mock.Of<IDiscordClientWrapper>();
        var player = Mock.Of<ILavalinkPlayer>(x => x.State == PlayerState.NotPlaying);

        var context = new InactivityTrackingContext(
            InactivityTrackingService: inactivityTrackingService,
            Client: discordClientWrapper,
            Player: player);

        var tracker = new IdleInactivityTracker();

        // Act
        var result = await tracker
            .CheckAsync(context)
            .ConfigureAwait(false);

        // Assert
        Assert.Equal(PlayerActivityStatus.Inactive, result);
    }*/
}
