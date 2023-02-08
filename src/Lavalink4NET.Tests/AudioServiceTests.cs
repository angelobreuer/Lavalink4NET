namespace Lavalink4NET.Tests;

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Integrations;
using Lavalink4NET.Players;
using Lavalink4NET.Protocol.Payloads;
using Lavalink4NET.Rest;
using Lavalink4NET.Socket;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public sealed class AudioServiceTests
{
    [Fact]
    public async Task TestWaitForReadyThrowsTimeoutExceptionAfterReadyTimeoutAsync()
    {
        var discordClientMock = new Mock<IDiscordClientWrapper>();
        var socketMock = new Mock<ILavalinkSocket>();

        var socketFactory = Mock.Of<ILavalinkSocketFactory>(
            x => x.Create(It.IsAny<IOptions<LavalinkSocketOptions>>()) == socketMock);

        discordClientMock
            .Setup(x => x.WaitForReadyAsync(It.IsAny<CancellationToken>()))
            .Returns((CancellationToken cancellationToken) => new ValueTask<ClientInformation>(new TaskCompletionSource<ClientInformation>().Task.WaitAsync(cancellationToken)));

        var audioService = new AudioService(
            serviceProvider: Mock.Of<IServiceProvider>(),
            discordClient: discordClientMock.Object,
            apiClient: Mock.Of<ILavalinkApiClient>(),
            playerManager: Mock.Of<IPlayerManager>(),
            trackManager: Mock.Of<ITrackManager>(),
            socketFactory: socketFactory,
            integrationManager: Mock.Of<IIntegrationManager>(),
            loggerFactory: NullLoggerFactory.Instance,
            options: Options.Create(new LavalinkNodeOptions { ReadyTimeout = TimeSpan.FromSeconds(1), }));

        // Act
        async Task Action()
        {
            await audioService!
                .WaitForReadyAsync()
                .ConfigureAwait(false);
        }

        // Assert
        await Assert
            .ThrowsAsync<TimeoutException>(Action)
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task TestWaitForReadyThrowsTaskCancelledExceptionWhenCancelledExternallyAsync()
    {
        var socketMock = new Mock<ILavalinkSocket>();
        var discordClientMock = new Mock<IDiscordClientWrapper>();

        var socketFactory = Mock.Of<ILavalinkSocketFactory>(
            x => x.Create(It.IsAny<IOptions<LavalinkSocketOptions>>()) == socketMock);

        discordClientMock
            .Setup(x => x.WaitForReadyAsync(It.IsAny<CancellationToken>()))
            .Returns((CancellationToken cancellationToken) => new ValueTask<ClientInformation>(new TaskCompletionSource<ClientInformation>().Task.WaitAsync(cancellationToken)));

        var audioService = new AudioService(
            serviceProvider: Mock.Of<IServiceProvider>(),
            discordClient: discordClientMock.Object,
            apiClient: Mock.Of<ILavalinkApiClient>(),
            playerManager: Mock.Of<IPlayerManager>(),
            trackManager: Mock.Of<ITrackManager>(),
            socketFactory: socketFactory,
            integrationManager: Mock.Of<IIntegrationManager>(),
            loggerFactory: NullLoggerFactory.Instance,
            options: Options.Create(new LavalinkNodeOptions { ReadyTimeout = Timeout.InfiniteTimeSpan, }));

        // Act
        async Task Action()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(0.5));

            await audioService!
                .WaitForReadyAsync(cancellationTokenSource.Token)
                .ConfigureAwait(false);
        }

        // Assert
        await Assert
            .ThrowsAsync<TaskCanceledException>(Action)
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task TestWaitForReadyTimesOutIfClientNotReadyDispatchedWithinTimeAsync()
    {
        var socketMock = new Mock<ILavalinkSocket>();
        var apiClientMock = new Mock<ILavalinkApiClient>();
        var discordClientMock = new Mock<IDiscordClientWrapper>();

        apiClientMock
            .SetupGet(x => x.Endpoints)
            .Returns(new LavalinkApiEndpoints(new Uri("http://localhost/")));

        var receiveChannel = Channel.CreateUnbounded<IPayload>();

        var socketFactory = Mock.Of<ILavalinkSocketFactory>(
            x => x.Create(It.IsAny<IOptions<LavalinkSocketOptions>>()) == socketMock.Object);

        socketMock
            .Setup(x => x.ReceiveAsync(It.IsAny<CancellationToken>()))
            .Returns(() => receiveChannel.Reader.ReadAsync());

        receiveChannel.Writer.TryWrite(new ReadyPayload(
            SessionResumed: false,
            SessionId: "session"));

        discordClientMock
            .Setup(x => x.WaitForReadyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClientInformation("Mock Client", 0UL, 1));

        var audioService = new AudioService(
            serviceProvider: Mock.Of<IServiceProvider>(),
            discordClient: discordClientMock.Object,
            apiClient: apiClientMock.Object,
            playerManager: Mock.Of<IPlayerManager>(),
            trackManager: Mock.Of<ITrackManager>(),
            socketFactory: socketFactory,
            integrationManager: Mock.Of<IIntegrationManager>(),
            loggerFactory: NullLoggerFactory.Instance,
            options: Options.Create(new LavalinkNodeOptions { ReadyTimeout = TimeSpan.FromSeconds(0.5), }));

        // Act
        await audioService!
            .WaitForReadyAsync()
            .ConfigureAwait(false);

        // Assert
        Assert.Equal("session", audioService.SessionId);
    }

    [Fact]
    public async Task TestIntegrationsCanInterceptPayloadAsync()
    {
        var apiClientMock = new Mock<ILavalinkApiClient>();
        var socketMock = new Mock<ILavalinkSocket>();
        var discordClientMock = new Mock<IDiscordClientWrapper>();

        apiClientMock
            .SetupGet(x => x.Endpoints)
            .Returns(new LavalinkApiEndpoints(new Uri("http://localhost/")));

        var receiveChannel = Channel.CreateUnbounded<IPayload>();

        var socketFactory = Mock.Of<ILavalinkSocketFactory>(
            x => x.Create(It.IsAny<IOptions<LavalinkSocketOptions>>()) == socketMock.Object);

        socketMock
            .Setup(x => x.ReceiveAsync(It.IsAny<CancellationToken>()))
            .Returns(() => receiveChannel.Reader.ReadAsync());

        receiveChannel.Writer.TryWrite(new ReadyPayload(
            SessionResumed: false,
            SessionId: "session"));

        discordClientMock
            .Setup(x => x.WaitForReadyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClientInformation("Mock Client", 0UL, 1));

        var integrationManager = new IntegrationManager();
        var integration = new Mock<ILavalinkIntegration>();

        integration
            .Setup(x => x.ProcessPayloadAsync(It.IsAny<IPayload>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask)
            .Verifiable();

        integrationManager.Set(integration.Object);

        var audioService = new AudioService(
            serviceProvider: Mock.Of<IServiceProvider>(),
            discordClient: discordClientMock.Object,
            apiClient: apiClientMock.Object,
            playerManager: Mock.Of<IPlayerManager>(),
            trackManager: Mock.Of<ITrackManager>(),
            socketFactory: socketFactory,
            integrationManager: integrationManager,
            loggerFactory: NullLoggerFactory.Instance,
            options: Options.Create(new LavalinkNodeOptions { ReadyTimeout = TimeSpan.FromSeconds(0.5), }));

        // Act
        await audioService!
            .WaitForReadyAsync()
            .ConfigureAwait(false);

        await Task.Delay(500);

        // Assert
        integration.Verify();
    }
}
