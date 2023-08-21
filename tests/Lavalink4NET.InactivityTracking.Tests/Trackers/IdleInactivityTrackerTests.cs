/*namespace Lavalink4NET.InactivityTracking.Tests.Trackers;

using Lavalink4NET.Clients;
using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;
using Moq;

public sealed class IdleInactivityTrackerTests
{
    [Fact]
    public async Task CheckWhenPlayerPlayingNotIdleAsync()
    {
        // Arrange
        var inactivityTrackingService = Mock.Of<IInactivityTrackingService>();
        var discordClientWrapper = Mock.Of<IDiscordClientWrapper>();
        var player = Mock.Of<ILavalinkPlayer>(x => x.State == PlayerState.Playing);

        var context = new InactivityTrackingContext(
            InactivityTrackingService: inactivityTrackingService,
            Client: discordClientWrapper);

        var tracker = new IdleInactivityTracker(IdleInactivityTrackerOptions.Default);

        // Act
        var result = await tracker
            .CheckAsync(context, player)
            .ConfigureAwait(false);

        // Assert
        Assert.Equal(PlayerActivityStatus.Active, result.Status);
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
            Client: discordClientWrapper);

        var tracker = new IdleInactivityTracker(IdleInactivityTrackerOptions.Default);

        // Act
        var result = await tracker
            .CheckAsync(context, player)
            .ConfigureAwait(false);

        // Assert
        Assert.Equal(PlayerActivityStatus.Inactive, result.Status);
    }
}
*/