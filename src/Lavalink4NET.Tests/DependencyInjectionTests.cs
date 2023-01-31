namespace Lavalink4NET.Tests;

using Lavalink4NET.Cluster;
using Lavalink4NET.Rest;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

/// <summary>
///     Tests the ability for dependency injection for various classes.
/// </summary>
public sealed class DependencyInjectionTests
{
    /// <summary>
    ///     Tests the ability for dependency injection for <see cref="LavalinkCluster"/>.
    /// </summary>
    [Fact]
    public void TestLavalinkClusterDI()
    {
        using var serviceProvider = new ServiceCollection()
            .AddSingleton<LavalinkClusterOptions>()
            .AddSingleton<IAudioService, LavalinkCluster>()
            .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
            .BuildServiceProvider();
        serviceProvider.GetRequiredService<IAudioService>();
    }

    /// <summary>
    ///     Tests the ability for dependency injection for <see cref="LavalinkCluster"/> with an
    ///     <see cref="IMemoryCache"/> service.
    /// </summary>
    [Fact]
    public void TestLavalinkClusterDIWithCache()
    {
        using var serviceProvider = new ServiceCollection()
            .AddSingleton<LavalinkClusterOptions>()
            .AddSingleton<IAudioService, LavalinkCluster>()
            .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
            .AddMemoryCache()
            .BuildServiceProvider();

        serviceProvider.GetRequiredService<IAudioService>();
    }

    /// <summary>
    ///     Tests the ability for dependency injection for <see cref="LavalinkCluster"/> with an
    ///     <see cref="IMemoryCache"/> service and <see cref="ILogger"/> service.
    /// </summary>
    [Fact]
    public void TestLavalinkClusterDIWithCacheAndLogger()
    {
        using var serviceProvider = new ServiceCollection()
            .AddSingleton<LavalinkClusterOptions>()
            .AddSingleton<IAudioService, LavalinkCluster>()
            .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
            .AddSingleton<ILoggerFactory, NullLoggerFactory>()
            .AddMemoryCache()
            .BuildServiceProvider();

        serviceProvider.GetRequiredService<IAudioService>();
    }

    /// <summary>
    ///     Tests the ability for dependency injection for <see cref="LavalinkCluster"/> with an
    ///     <see cref="ILogger"/> service.
    /// </summary>
    [Fact]
    public void TestLavalinkClusterDIWithLogger()
    {
        using var serviceProvider = new ServiceCollection()
            .AddSingleton<LavalinkClusterOptions>()
            .AddSingleton<IAudioService, LavalinkCluster>()
            .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
            .AddSingleton<ILoggerFactory, NullLoggerFactory>()
            .BuildServiceProvider();

        serviceProvider.GetRequiredService<IAudioService>();
    }

    /// <summary>
    ///     Tests the ability for dependency injection for <see cref="LavalinkNode"/>.
    /// </summary>
    [Fact]
    public void TestLavalinkNodeDI()
    {
        using var serviceProvider = new ServiceCollection()
            .AddSingleton<LavalinkNodeOptions>()
            .AddSingleton<IAudioService, LavalinkNode>()
            .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
            .BuildServiceProvider();

        serviceProvider.GetRequiredService<IAudioService>();
    }

    /// <summary>
    ///     Tests the ability for dependency injection for <see cref="LavalinkNode"/> with an
    ///     <see cref="IMemoryCache"/> service.
    /// </summary>
    [Fact]
    public void TestLavalinkNodeDIWithCache()
    {
        using var serviceProvider = new ServiceCollection()
            .AddSingleton<LavalinkNodeOptions>()
            .AddSingleton<IAudioService, LavalinkNode>()
            .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
            .AddMemoryCache()
            .BuildServiceProvider();

        serviceProvider.GetRequiredService<IAudioService>();
    }

    /// <summary>
    ///     Tests the ability for dependency injection for <see cref="LavalinkNode"/> with an
    ///     <see cref="ILogger"/> service.
    /// </summary>
    [Fact]
    public void TestLavalinkNodeDIWithLogger()
    {
        using var serviceProvider = new ServiceCollection()
            .AddSingleton<LavalinkNodeOptions>()
            .AddSingleton<IAudioService, LavalinkNode>()
            .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
            .AddSingleton<ILoggerFactory, NullLoggerFactory>()
            .BuildServiceProvider();
        serviceProvider.GetRequiredService<IAudioService>();
    }

    /// <summary>
    ///     Tests the ability for dependency injection for <see cref="LavalinkNode"/> with an
    ///     <see cref="ILogger"/> service and an <see cref="IMemoryCache"/> service.
    /// </summary>
    [Fact]
    public void TestLavalinkNodeDIWithLoggerAndCache()
    {
        using var serviceProvider = new ServiceCollection()
            .AddSingleton<LavalinkNodeOptions>()
            .AddSingleton<IAudioService, LavalinkNode>()
            .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
            .AddSingleton<ILoggerFactory, NullLoggerFactory>()
            .AddMemoryCache()
            .BuildServiceProvider();

        serviceProvider.GetRequiredService<IAudioService>();
    }

    /// <summary>
    ///     Tests the ability for dependency injection for <see cref="LavalinkRestClient"/>.
    /// </summary>
    [Fact]
    public void TestLavalinkRestClientDI()
    {
        using var serviceProvider = new ServiceCollection()
            .AddSingleton<LavalinkRestOptions>()
            .AddSingleton<ILavalinkRestClient, LavalinkRestClient>()
            .BuildServiceProvider();
        serviceProvider.GetRequiredService<ILavalinkRestClient>();
    }

    /// <summary>
    ///     Tests the ability for dependency injection for <see cref="LavalinkRestClient"/> with
    ///     an <see cref="IMemoryCache"/> service.
    /// </summary>
    [Fact]
    public void TestLavalinkRestClientDIWithCache()
    {
        using var serviceProvider = new ServiceCollection()
            .AddSingleton<LavalinkRestOptions>()
            .AddSingleton<ILavalinkRestClient, LavalinkRestClient>()
            .AddMemoryCache()
            .BuildServiceProvider();
        serviceProvider.GetRequiredService<ILavalinkRestClient>();
    }

    /// <summary>
    ///     Tests the ability for dependency injection for <see cref="LavalinkRestClient"/> with
    ///     an <see cref="ILogger"/> service and an <see cref="IMemoryCache"/> service.
    /// </summary>
    [Fact]
    public void TestLavalinkRestClientDIWithCacheAndLogger()
    {
        using var serviceProvider = new ServiceCollection()
            .AddSingleton<LavalinkRestOptions>()
            .AddSingleton<ILavalinkRestClient, LavalinkRestClient>()
            .AddMemoryCache()
            .AddSingleton<ILoggerFactory, NullLoggerFactory>()
            .BuildServiceProvider();
        serviceProvider.GetRequiredService<ILavalinkRestClient>();
    }

    /// <summary>
    ///     Tests the ability for dependency injection for <see cref="LavalinkRestClient"/> with
    ///     an <see cref="ILogger"/> service.
    /// </summary>
    [Fact]
    public void TestLavalinkRestClientDIWithLogger()
    {
        using var serviceProvider = new ServiceCollection()
            .AddSingleton<LavalinkRestOptions>()
            .AddSingleton<ILavalinkRestClient, LavalinkRestClient>()
            .AddSingleton<ILoggerFactory, NullLoggerFactory>()
            .BuildServiceProvider();

        serviceProvider.GetRequiredService<ILavalinkRestClient>();
    }
}
