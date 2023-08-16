namespace Lavalink4NET.Tests.Players;

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Clients.Events;
using Lavalink4NET.Events;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Preconditions;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Protocol.Requests;
using Lavalink4NET.Rest;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public sealed class PlayerManagerTests
{
    [Fact]
    public void TestPlayerManagerRegistersEvents()
    {
        // Arrange
        var discordClient = new EventRegistrationDiscordClientWrapper();

        // Act
        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: discordClient,
            sessionProvider: Mock.Of<ILavalinkSessionProvider>(),
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        // Assert
        Assert.True(discordClient.HasVoiceServerUpdatedRegistration);
        Assert.True(discordClient.HasVoiceStateUpdatedRegistration);
    }

    [Fact]
    public void TestPlayerManagerHasNoPlayersInitially()
    {
        // Act
        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: Mock.Of<IDiscordClientWrapper>(),
            sessionProvider: Mock.Of<ILavalinkSessionProvider>(),
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        // Assert
        Assert.Empty(playerManager.Players);
    }

    [Fact]
    public async Task TestPlayerManagerCreatesPlayer()
    {
        // Arrange
        var model = new PlayerInformationModel(GuildId: 0, CurrentTrack: null, Volume: 1F, IsPaused: false, VoiceState: null!, Filters: null!);

        var apiClient = Mock.Of<ILavalinkApiClient>(x
            => x.UpdatePlayerAsync(It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<PlayerUpdateProperties>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(model));

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        // Act
        var player = await playerManager.JoinAsync(
            guildId: 0,
            voiceChannelId: 0,
            playerFactory: PlayerFactory.Default,
            options: Options.Create(new LavalinkPlayerOptions()));

        // Assert
        Assert.NotNull(player);
        Assert.Single(playerManager.Players);
    }

    [Fact]
    public async Task TestPlayerManagerDoesNotReturnDestroyedPlayers()
    {
        // Arrange
        var model = new PlayerInformationModel(GuildId: 0, CurrentTrack: null, Volume: 1F, IsPaused: false, VoiceState: null!, Filters: null!);

        var apiClient = Mock.Of<ILavalinkApiClient>(x
            => x.UpdatePlayerAsync(It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<PlayerUpdateProperties>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(model));

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        // Act
        var player = await playerManager.JoinAsync(
            guildId: 0,
            voiceChannelId: 0,
            playerFactory: PlayerFactory.Default,
            options: Options.Create(new LavalinkPlayerOptions()));

        await player.DisposeAsync().ConfigureAwait(false);

        // Assert
        Assert.Empty(playerManager.Players);
    }

    [Fact]
    public async Task TestGetPlayerReturnsPlayerFromJoinAsync()
    {
        // Arrange
        var model = new PlayerInformationModel(GuildId: 0, CurrentTrack: null, Volume: 1F, IsPaused: false, VoiceState: null!, Filters: null!);

        var apiClient = Mock.Of<ILavalinkApiClient>(x
            => x.UpdatePlayerAsync(It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<PlayerUpdateProperties>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(model));

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        var joinedPlayer = await playerManager.JoinAsync(
            guildId: 0,
            voiceChannelId: 0,
            playerFactory: PlayerFactory.Default,
            options: Options.Create(new LavalinkPlayerOptions()));

        // Act
        var player = await playerManager.GetPlayerAsync(guildId: 0);

        // Assert
        Assert.Same(joinedPlayer, player);
    }

    [Fact]
    public async Task TestGetPlayerThrowsIfPlayerTypeDoesNotMatchAsync()
    {
        // Arrange
        var model = new PlayerInformationModel(GuildId: 0, CurrentTrack: null, Volume: 1F, IsPaused: false, VoiceState: null!, Filters: null!);

        var apiClient = Mock.Of<ILavalinkApiClient>(x
            => x.UpdatePlayerAsync(It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<PlayerUpdateProperties>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(model));

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        await playerManager.JoinAsync(
            guildId: 0,
            voiceChannelId: 0,
            playerFactory: PlayerFactory.Default,
            options: Options.Create(new LavalinkPlayerOptions()));

        // Act
        async Task Act() => await playerManager
            .GetPlayerAsync<QueuedLavalinkPlayer>(guildId: 0)
            .ConfigureAwait(false);

        // Assert
        await Assert
            .ThrowsAsync<InvalidOperationException>(Act)
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task TestGetPlayerReturnsTypedPlayerAsync()
    {
        // Arrange
        var model = new PlayerInformationModel(GuildId: 0, CurrentTrack: null, Volume: 1F, IsPaused: false, VoiceState: null!, Filters: null!);

        var apiClient = Mock.Of<ILavalinkApiClient>(x
            => x.UpdatePlayerAsync(It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<PlayerUpdateProperties>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(model));

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        var joinedPlayer = await playerManager.JoinAsync(
            guildId: 0,
            voiceChannelId: 0,
            playerFactory: PlayerFactory.Queued,
            options: Options.Create(new QueuedLavalinkPlayerOptions()));

        // Act
        var player = await playerManager
            .GetPlayerAsync<QueuedLavalinkPlayer>(guildId: 0)
            .ConfigureAwait(false);

        // Assert
        Assert.Same(joinedPlayer, player);
    }

    [Fact]
    public async Task TestGetPlayersReturnsTypedPlayersAsync()
    {
        // Arrange
        var model = new PlayerInformationModel(GuildId: 0, CurrentTrack: null, Volume: 1F, IsPaused: false, VoiceState: null!, Filters: null!);

        var apiClient = Mock.Of<ILavalinkApiClient>(x
            => x.UpdatePlayerAsync(It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<PlayerUpdateProperties>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(model));

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        var joinedPlayer = await playerManager.JoinAsync(
            guildId: 0,
            voiceChannelId: 0,
            playerFactory: PlayerFactory.Queued,
            options: Options.Create(new QueuedLavalinkPlayerOptions()));

        // Act
        var players = playerManager.GetPlayers<QueuedLavalinkPlayer>();

        // Assert
        var player = Assert.Single(players);
        Assert.Same(joinedPlayer, player);
    }

    [Fact]
    public async Task TestGetPlayerReturnsNullIfPlayerNotFoundAsync()
    {
        // Arrange
        var apiClient = Mock.Of<ILavalinkApiClient>();

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        // Act
        var player = await playerManager.GetPlayerAsync(guildId: 0);

        // Assert
        Assert.Null(player);
    }

    [Fact]
    public void TestHasPlayerFalseIfGuildHasNoPlayer()
    {
        // Arrange
        var apiClient = Mock.Of<ILavalinkApiClient>();

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        // Act
        var hasPlayer = playerManager.HasPlayer(guildId: 0UL);

        // Assert
        Assert.False(hasPlayer);
    }

    [Fact]
    public async Task TestHasPlayerTrueIfGuildHasPlayerAsync()
    {
        // Arrange
        var model = new PlayerInformationModel(GuildId: 0, CurrentTrack: null, Volume: 1F, IsPaused: false, VoiceState: null!, Filters: null!);

        var apiClient = Mock.Of<ILavalinkApiClient>(x
            => x.UpdatePlayerAsync(It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<PlayerUpdateProperties>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(model));

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        await playerManager.JoinAsync(
            guildId: 0,
            voiceChannelId: 0,
            playerFactory: PlayerFactory.Queued,
            options: Options.Create(new QueuedLavalinkPlayerOptions()));

        // Act
        var hasPlayer = playerManager.HasPlayer(guildId: 0UL);

        // Assert
        Assert.True(hasPlayer);
    }

    [Fact]
    public async Task TestHasPlayerFalseIfGuildHasPlayerButPlayerIsDestroyedAsync()
    {
        // Arrange
        var model = new PlayerInformationModel(GuildId: 0, CurrentTrack: null, Volume: 1F, IsPaused: false, VoiceState: null!, Filters: null!);

        var apiClient = Mock.Of<ILavalinkApiClient>(x
            => x.UpdatePlayerAsync(It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<PlayerUpdateProperties>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(model));

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        var player = await playerManager.JoinAsync(
            guildId: 0,
            voiceChannelId: 0,
            playerFactory: PlayerFactory.Queued,
            options: Options.Create(new QueuedLavalinkPlayerOptions()));

        await player.DisposeAsync().ConfigureAwait(false);

        // Act
        var hasPlayer = playerManager.HasPlayer(guildId: 0UL);

        // Assert
        Assert.False(hasPlayer);
    }

    [Fact]
    public async Task TestSimpleRetrieveAsync()
    {
        // Arrange
        var model = new PlayerInformationModel(GuildId: 0, CurrentTrack: null, Volume: 1F, IsPaused: false, VoiceState: null!, Filters: null!);

        var apiClient = Mock.Of<ILavalinkApiClient>(x
            => x.UpdatePlayerAsync(It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<PlayerUpdateProperties>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(model));

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: PlayerChannelBehavior.Join);

        // Act
        var result = await playerManager.RetrieveAsync(
            guildId: 0,
            memberVoiceChannel: 0,
            playerFactory: PlayerFactory.Queued,
            retrieveOptions: retrieveOptions,
            options: Options.Create(new QueuedLavalinkPlayerOptions()));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Player);
    }

    [Fact]
    public async Task TestSimpleRetrieveWithoutJoinFailsWithBotNotConnectedAsync()
    {
        // Arrange
        var model = new PlayerInformationModel(GuildId: 0, CurrentTrack: null, Volume: 1F, IsPaused: false, VoiceState: null!, Filters: null!);

        var apiClient = Mock.Of<ILavalinkApiClient>(x
            => x.UpdatePlayerAsync(It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<PlayerUpdateProperties>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(model));

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: PlayerChannelBehavior.None);

        // Act
        var result = await playerManager.RetrieveAsync(
            guildId: 0,
            memberVoiceChannel: 0,
            playerFactory: PlayerFactory.Queued,
            retrieveOptions: retrieveOptions,
            options: Options.Create(new QueuedLavalinkPlayerOptions()));

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(PlayerRetrieveStatus.BotNotConnected, result.Status);
    }

    [Fact]
    public async Task TestRetrieveWithFailingPreconditionAsync()
    {
        // Arrange
        var model = new PlayerInformationModel(GuildId: 0, CurrentTrack: null, Volume: 1F, IsPaused: false, VoiceState: null!, Filters: null!);

        var apiClient = Mock.Of<ILavalinkApiClient>(x
            => x.UpdatePlayerAsync(It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<PlayerUpdateProperties>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(model));

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: PlayerChannelBehavior.Join,
            Preconditions: ImmutableArray.Create(PlayerPrecondition.QueueNotEmpty));

        // Act
        var result = await playerManager.RetrieveAsync(
            guildId: 0,
            memberVoiceChannel: 0,
            playerFactory: PlayerFactory.Queued,
            retrieveOptions: retrieveOptions,
            options: Options.Create(new QueuedLavalinkPlayerOptions()));

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(PlayerRetrieveStatus.PreconditionFailed, result.Status);
        Assert.Equal(PlayerPrecondition.QueueNotEmpty, result.Precondition);
    }

    [Fact]
    public async Task TestRetrieveWithNonFailingPreconditionAsync()
    {
        // Arrange
        var model = new PlayerInformationModel(GuildId: 0, CurrentTrack: null, Volume: 1F, IsPaused: false, VoiceState: null!, Filters: null!);

        var apiClient = Mock.Of<ILavalinkApiClient>(x
            => x.UpdatePlayerAsync(It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<PlayerUpdateProperties>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(model));

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: PlayerChannelBehavior.Join,
            Preconditions: ImmutableArray.Create(PlayerPrecondition.QueueEmpty));

        // Act
        var result = await playerManager.RetrieveAsync(
            guildId: 0,
            memberVoiceChannel: 0,
            playerFactory: PlayerFactory.Queued,
            retrieveOptions: retrieveOptions,
            options: Options.Create(new QueuedLavalinkPlayerOptions()));

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task TestRetrieveWithJoinWithoutMemberChannelAsync()
    {
        // Arrange
        var model = new PlayerInformationModel(GuildId: 0, CurrentTrack: null, Volume: 1F, IsPaused: false, VoiceState: null!, Filters: null!);

        var apiClient = Mock.Of<ILavalinkApiClient>(x
            => x.UpdatePlayerAsync(It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<PlayerUpdateProperties>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(model));

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: PlayerChannelBehavior.Join,
            Preconditions: ImmutableArray.Create(PlayerPrecondition.QueueNotEmpty));

        // Act
        var result = await playerManager.RetrieveAsync(
            guildId: 0,
            memberVoiceChannel: null,
            playerFactory: PlayerFactory.Queued,
            retrieveOptions: retrieveOptions,
            options: Options.Create(new QueuedLavalinkPlayerOptions()));

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(PlayerRetrieveStatus.UserNotInVoiceChannel, result.Status);
    }

    [Fact]
    public async Task TestRetrieveWithMemberVoiceStateAlwaysRequiredFailsAsync()
    {
        // Arrange
        var model = new PlayerInformationModel(GuildId: 0, CurrentTrack: null, Volume: 1F, IsPaused: false, VoiceState: null!, Filters: null!);

        var apiClient = Mock.Of<ILavalinkApiClient>(x
            => x.UpdatePlayerAsync(It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<PlayerUpdateProperties>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(model));

        var sessionProvider = Mock.Of<ILavalinkSessionProvider>(x
            => x.GetSessionAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>())
            == ValueTask.FromResult(new LavalinkPlayerSession(apiClient, "abc", "abc")));

        using var playerManager = new PlayerManager(
            serviceProvider: null,
            discordClient: new VoiceEmulatingDiscordClientWrapper(),
            sessionProvider: sessionProvider,
            systemClock: new SystemClock(),
            loggerFactory: NullLoggerFactory.Instance);

        var initial
            RetrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: PlayerChannelBehavior.Join,
            VoiceStateBehavior: MemberVoiceStateBehavior.AlwaysRequired,
            Preconditions: ImmutableArray.Create(PlayerPrecondition.QueueNotEmpty));

        await playerManager.RetrieveAsync(
           guildId: 0,
           memberVoiceChannel: null,
           playerFactory: PlayerFactory.Queued,
           retrieveOptions: default,
           options: Options.Create(new QueuedLavalinkPlayerOptions()));

        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: PlayerChannelBehavior.Join,
            VoiceStateBehavior: MemberVoiceStateBehavior.AlwaysRequired,
            Preconditions: ImmutableArray.Create(PlayerPrecondition.QueueNotEmpty));

        // Act
        var result = await playerManager.RetrieveAsync(
            guildId: 0,
            memberVoiceChannel: null,
            playerFactory: PlayerFactory.Queued,
            retrieveOptions: retrieveOptions,
            options: Options.Create(new QueuedLavalinkPlayerOptions()));

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(PlayerRetrieveStatus.UserNotInVoiceChannel, result.Status);
    }
}

file sealed class VoiceEmulatingDiscordClientWrapper : IDiscordClientWrapper
{
    public event AsyncEventHandler<VoiceServerUpdatedEventArgs>? VoiceServerUpdated;
    public event AsyncEventHandler<VoiceStateUpdatedEventArgs>? VoiceStateUpdated;

    public ValueTask<ImmutableArray<ulong>> GetChannelUsersAsync(ulong guildId, ulong voiceChannelId, bool includeBots = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async ValueTask SendVoiceUpdateAsync(ulong guildId, ulong? voiceChannelId, bool selfDeaf = false, bool selfMute = false, CancellationToken cancellationToken = default)
    {
        var voiceStateUpdatedEventArgs = new VoiceStateUpdatedEventArgs(
            guildId: guildId,
            voiceState: new VoiceState(voiceChannelId, "abc"));

        await VoiceStateUpdated
            .InvokeAsync(this, voiceStateUpdatedEventArgs)
            .ConfigureAwait(false);

        var voiceServerUpdatedEventArgs = new VoiceServerUpdatedEventArgs(
            guildId: guildId,
            voiceServer: new VoiceServer("token", "wss.endpoint.com"));

        await VoiceServerUpdated
            .InvokeAsync(this, voiceServerUpdatedEventArgs)
            .ConfigureAwait(false);
    }

    public ValueTask<ClientInformation> WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

file sealed class EventRegistrationDiscordClientWrapper : IDiscordClientWrapper
{
    public event AsyncEventHandler<VoiceServerUpdatedEventArgs>? VoiceServerUpdated;

    public event AsyncEventHandler<VoiceStateUpdatedEventArgs>? VoiceStateUpdated;

    public bool HasVoiceServerUpdatedRegistration => VoiceServerUpdated is not null;

    public bool HasVoiceStateUpdatedRegistration => VoiceStateUpdated is not null;

    public ValueTask<ImmutableArray<ulong>> GetChannelUsersAsync(ulong guildId, ulong voiceChannelId, bool includeBots = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask SendVoiceUpdateAsync(ulong guildId, ulong? voiceChannelId, bool selfDeaf = false, bool selfMute = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<ClientInformation> WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
