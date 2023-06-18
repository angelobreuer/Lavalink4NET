namespace Lavalink4NET.Tests.Players;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Players;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Protocol.Models.Filters;
using Lavalink4NET.Protocol.Requests;
using Lavalink4NET.Rest;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public sealed class LavalinkPlayerHandleTests
{
    [Fact]
    public async Task TestGetPlayerReturnsWhenCompletedAsync()
    {
        // Arrange
        var handle = new LavalinkPlayerHandle<LavalinkPlayer, LavalinkPlayerOptions>(
            guildId: 123UL,
            playerContext: CreatePlayerContext(),
            playerFactory: PlayerFactory.Default,
            options: Options.Create(new LavalinkPlayerOptions()),
            logger: NullLogger<LavalinkPlayer>.Instance);

        var task = handle.GetPlayerAsync().AsTask();

        Assert.False(task.IsCompleted);

        // Act
        await handle.UpdateVoiceServerAsync(new VoiceServer("abc", "abc.discord.gg"));
        await handle.UpdateVoiceStateAsync(new VoiceState(123, "abc"));

        // Assert
        Assert.True(task.IsCompleted);
    }

    [Fact]
    public async Task TestCanGetPlayerSyncAfterCompleteAsync()
    {
        // Arrange
        var handle = new LavalinkPlayerHandle<LavalinkPlayer, LavalinkPlayerOptions>(
            guildId: 123UL,
            playerContext: CreatePlayerContext(),
            playerFactory: PlayerFactory.Default,
            options: Options.Create(new LavalinkPlayerOptions()),
            logger: NullLogger<LavalinkPlayer>.Instance);

        Assert.Null(handle.Player);

        // Act
        await handle.UpdateVoiceServerAsync(new VoiceServer("abc", "abc.discord.gg"));
        await handle.UpdateVoiceStateAsync(new VoiceState(123, "abc"));

        // Assert
        Assert.NotNull(handle.Player);
    }

    private static ILavalinkApiClient CreateApiClientMock()
    {
        var apiClient = new Mock<ILavalinkApiClient>();

        apiClient
            .Setup(x => x.UpdatePlayerAsync(It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<PlayerUpdateProperties>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string sessionId, ulong guildId, PlayerUpdateProperties properties, CancellationToken cancellationToken) =>
            {
                var voiceState = properties.VoiceState.Value;

                return new PlayerInformationModel(
                    GuildId: guildId,
                    CurrentTrack: null,
                    Volume: properties.Volume.GetValueOrDefault(1F),
                    IsPaused: properties.IsPaused.GetValueOrDefault(false),
                    VoiceState: new VoiceStateModel(
                        Token: voiceState.Token,
                        Endpoint: voiceState.Endpoint,
                        SessionId: voiceState.SessionId,
                        IsConnected: true,
                        Latency: null),
                    Filters: new PlayerFilterMapModel());
            });

        return apiClient.Object;
    }

    private static PlayerContext CreatePlayerContext() => new(
        ServiceProvider: null,
        ApiClient: CreateApiClientMock(),
        DiscordClient: Mock.Of<IDiscordClientWrapper>(),
        SessionProvider: Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionIdAsync(It.IsAny<CancellationToken>())
            == ValueTask.FromResult("abc")),
        SystemClock: new SystemClock());
}
