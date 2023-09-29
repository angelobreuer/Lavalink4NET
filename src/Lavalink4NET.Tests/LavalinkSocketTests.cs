namespace Lavalink4NET.Tests;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Protocol.Payloads;
using Lavalink4NET.Socket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

public sealed class LavalinkSocketTests
{
    [Fact]
    public async Task TestAuthorizationHeaderMatchesPassphraseAsync()
    {
        // Arrange
        var socketFactory = new LavalinkSocketFactory(
            httpMessageHandlerFactory: new HttpMessageHandlerFactory(),
            reconnectStrategy: new ExponentialBackoffReconnectStrategy(
                options: Options.Create(new ExponentialBackoffReconnectStrategyOptions())),
            systemClock: new SystemClock(),
            logger: NullLogger<LavalinkSocket>.Instance);

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddWebSockets(_ => { });
        builder.WebHost.UseKestrel(x => x.Listen(IPAddress.Loopback, 0));

        var webHost = builder.Build();
        await using var __ = webHost.ConfigureAwait(false);

        using var cancellationTokenSource = new CancellationTokenSource(3000);
        var taskCompletionSource = new TaskCompletionSource();

        webHost.UseWebSockets();

        webHost.MapGet("/", async (HttpContext httpContext) =>
        {
            using var webSocket = await httpContext.WebSockets
                .AcceptWebSocketAsync()
                .ConfigureAwait(false);

            Verify(httpContext);

            cancellationTokenSource.Cancel();
            taskCompletionSource.TrySetResult();
        });

        webHost.Start();

        var options = new LavalinkSocketOptions
        {
            Passphrase = "abc",
            Uri = new UriBuilder(webHost.Urls.First()) { Scheme = "ws", }.Uri,
        };

        // Act
        using var socket = socketFactory.Create(Options.Create(options));
        _ = socket.RunAsync(cancellationTokenSource.Token).AsTask();
        await taskCompletionSource.Task.ConfigureAwait(false);

        // Assert
        static void Verify(HttpContext httpContext)
        {
            Assert.Equal("abc", httpContext.Request.Headers["Authorization"]);
        }
    }

    [Fact]
    public async Task TestReceiveAsync()
    {
        // Arrange
        var socketFactory = new LavalinkSocketFactory(
            httpMessageHandlerFactory: new HttpMessageHandlerFactory(),
            reconnectStrategy: new ExponentialBackoffReconnectStrategy(
                options: Options.Create(new ExponentialBackoffReconnectStrategyOptions())),
            systemClock: new SystemClock(),
            logger: NullLogger<LavalinkSocket>.Instance);

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddWebSockets(_ => { });
        builder.WebHost.UseKestrel(x => x.Listen(IPAddress.Loopback, 0));

        var webHost = builder.Build();
        await using var __ = webHost.ConfigureAwait(false);

        webHost.UseWebSockets();

        webHost.MapGet("/", async (HttpContext httpContext) =>
        {
            using var webSocket = await httpContext.WebSockets
                .AcceptWebSocketAsync()
                .ConfigureAwait(false);

            var payload = """{"op": "ready","resumed": false,"sessionId": "asdf"}"""u8.ToArray();

            await webSocket
                .SendAsync(payload, WebSocketMessageType.Text, true, CancellationToken.None)
                .ConfigureAwait(false);
        });

        webHost.Start();

        var options = new LavalinkSocketOptions
        {
            Passphrase = "abc",
            Uri = new UriBuilder(webHost.Urls.First()) { Scheme = "ws", }.Uri,
        };

        using var socket = socketFactory.Create(Options.Create(options));
        _ = socket.RunAsync().AsTask();

        // Act
        var payload = await socket
            .ReceiveAsync()
            .ConfigureAwait(false);

        // Assert
        Assert.IsType<ReadyPayload>(payload);
    }
}

file sealed class HttpMessageHandlerFactory : IHttpMessageHandlerFactory
{
    public HttpMessageHandler CreateHandler(string name) => new HttpClientHandler();
}