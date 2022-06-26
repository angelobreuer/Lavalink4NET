/*
 *  File:   DependencyInjectionTests.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2022
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */

namespace Lavalink4NET.Tests
{
    using Lavalink4NET.Cluster;
    using Lavalink4NET.Logging;
    using Lavalink4NET.Lyrics;
    using Lavalink4NET.MemoryCache;
    using Lavalink4NET.Rest;
    using Microsoft.Extensions.DependencyInjection;
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
            using (var serviceProvider = new ServiceCollection()
                .AddSingleton<LavalinkClusterOptions>()
                .AddSingleton<IAudioService, LavalinkCluster>()
                .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
                .BuildServiceProvider())
            {
                serviceProvider.GetRequiredService<IAudioService>();
            }
        }

        /// <summary>
        ///     Tests the ability for dependency injection for <see cref="LavalinkCluster"/> with an
        ///     <see cref="ILavalinkCache"/> service.
        /// </summary>
        [Fact]
        public void TestLavalinkClusterDIWithCache()
        {
            using (var serviceProvider = new ServiceCollection()
                .AddSingleton<LavalinkClusterOptions>()
                .AddSingleton<IAudioService, LavalinkCluster>()
                .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
                .AddSingleton<ILavalinkCache, LavalinkCache>()
                .BuildServiceProvider())
            {
                serviceProvider.GetRequiredService<IAudioService>();
            }
        }

        /// <summary>
        ///     Tests the ability for dependency injection for <see cref="LavalinkCluster"/> with an
        ///     <see cref="ILavalinkCache"/> service and <see cref="ILogger"/> service.
        /// </summary>
        [Fact]
        public void TestLavalinkClusterDIWithCacheAndLogger()
        {
            using (var serviceProvider = new ServiceCollection()
                .AddSingleton<LavalinkClusterOptions>()
                .AddSingleton<IAudioService, LavalinkCluster>()
                .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
                .AddSingleton<ILogger, NullLogger>()
                .AddSingleton<ILavalinkCache, LavalinkCache>()
                .BuildServiceProvider())
            {
                serviceProvider.GetRequiredService<IAudioService>();
            }
        }

        /// <summary>
        ///     Tests the ability for dependency injection for <see cref="LavalinkCluster"/> with an
        ///     <see cref="ILogger"/> service.
        /// </summary>
        [Fact]
        public void TestLavalinkClusterDIWithLogger()
        {
            using (var serviceProvider = new ServiceCollection()
                .AddSingleton<LavalinkClusterOptions>()
                .AddSingleton<IAudioService, LavalinkCluster>()
                .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
                .AddSingleton<ILogger, NullLogger>()
                .BuildServiceProvider())
            {
                serviceProvider.GetRequiredService<IAudioService>();
            }
        }

        /// <summary>
        ///     Tests the ability for dependency injection for <see cref="LavalinkNode"/>.
        /// </summary>
        [Fact]
        public void TestLavalinkNodeDI()
        {
            using (var serviceProvider = new ServiceCollection()
                .AddSingleton<LavalinkNodeOptions>()
                .AddSingleton<IAudioService, LavalinkNode>()
                .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
                .BuildServiceProvider())
            {
                serviceProvider.GetRequiredService<IAudioService>();
            }
        }

        /// <summary>
        ///     Tests the ability for dependency injection for <see cref="LavalinkNode"/> with an
        ///     <see cref="ILavalinkCache"/> service.
        /// </summary>
        [Fact]
        public void TestLavalinkNodeDIWithCache()
        {
            using (var serviceProvider = new ServiceCollection()
                .AddSingleton<LavalinkNodeOptions>()
                .AddSingleton<IAudioService, LavalinkNode>()
                .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
                .AddSingleton<ILavalinkCache, LavalinkCache>()
                .BuildServiceProvider())
            {
                serviceProvider.GetRequiredService<IAudioService>();
            }
        }

        /// <summary>
        ///     Tests the ability for dependency injection for <see cref="LavalinkNode"/> with an
        ///     <see cref="ILogger"/> service.
        /// </summary>
        [Fact]
        public void TestLavalinkNodeDIWithLogger()
        {
            using (var serviceProvider = new ServiceCollection()
                .AddSingleton<LavalinkNodeOptions>()
                .AddSingleton<IAudioService, LavalinkNode>()
                .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
                .AddSingleton<ILogger, NullLogger>()
                .BuildServiceProvider())
            {
                serviceProvider.GetRequiredService<IAudioService>();
            }
        }

        /// <summary>
        ///     Tests the ability for dependency injection for <see cref="LavalinkNode"/> with an
        ///     <see cref="ILogger"/> service and an <see cref="ILavalinkCache"/> service.
        /// </summary>
        [Fact]
        public void TestLavalinkNodeDIWithLoggerAndCache()
        {
            using (var serviceProvider = new ServiceCollection()
                .AddSingleton<LavalinkNodeOptions>()
                .AddSingleton<IAudioService, LavalinkNode>()
                .AddSingleton<IDiscordClientWrapper, NullDiscordClientWrapper>()
                .AddSingleton<ILogger, NullLogger>()
                .AddSingleton<ILavalinkCache, LavalinkCache>()
                .BuildServiceProvider())
            {
                serviceProvider.GetRequiredService<IAudioService>();
            }
        }

        /// <summary>
        ///     Tests the ability for dependency injection for <see cref="LavalinkRestClient"/>.
        /// </summary>
        [Fact]
        public void TestLavalinkRestClientDI()
        {
            using (var serviceProvider = new ServiceCollection()
                .AddSingleton<LavalinkRestOptions>()
                .AddSingleton<ILavalinkRestClient, LavalinkRestClient>()
                .BuildServiceProvider())
            {
                serviceProvider.GetRequiredService<ILavalinkRestClient>();
            }
        }

        /// <summary>
        ///     Tests the ability for dependency injection for <see cref="LavalinkRestClient"/> with
        ///     an <see cref="ILavalinkCache"/> service.
        /// </summary>
        [Fact]
        public void TestLavalinkRestClientDIWithCache()
        {
            using (var serviceProvider = new ServiceCollection()
                .AddSingleton<LavalinkRestOptions>()
                .AddSingleton<ILavalinkRestClient, LavalinkRestClient>()
                .AddSingleton<ILavalinkCache, LavalinkCache>()
                .BuildServiceProvider())
            {
                serviceProvider.GetRequiredService<ILavalinkRestClient>();
            }
        }

        /// <summary>
        ///     Tests the ability for dependency injection for <see cref="LavalinkRestClient"/> with
        ///     an <see cref="ILogger"/> service and an <see cref="ILavalinkCache"/> service.
        /// </summary>
        [Fact]
        public void TestLavalinkRestClientDIWithCacheAndLogger()
        {
            using (var serviceProvider = new ServiceCollection()
                .AddSingleton<LavalinkRestOptions>()
                .AddSingleton<ILavalinkRestClient, LavalinkRestClient>()
                .AddSingleton<ILavalinkCache, LavalinkCache>()
                .AddSingleton<ILogger, NullLogger>()
                .BuildServiceProvider())
            {
                serviceProvider.GetRequiredService<ILavalinkRestClient>();
            }
        }

        /// <summary>
        ///     Tests the ability for dependency injection for <see cref="LavalinkRestClient"/> with
        ///     an <see cref="ILogger"/> service.
        /// </summary>
        [Fact]
        public void TestLavalinkRestClientDIWithLogger()
        {
            using (var serviceProvider = new ServiceCollection()
                .AddSingleton<LavalinkRestOptions>()
                .AddSingleton<ILavalinkRestClient, LavalinkRestClient>()
                .AddSingleton<ILogger, NullLogger>()
                .BuildServiceProvider())
            {
                serviceProvider.GetRequiredService<ILavalinkRestClient>();
            }
        }

        /// <summary>
        ///     Tests the ability for dependency injection for <see cref="LyricsService"/> with an
        ///     <see cref="ILavalinkCache"/> service.
        /// </summary>
        [Fact]
        public void TestLyricsServiceDI()
        {
            using (var serviceProvider = new ServiceCollection()
                .AddSingleton<LyricsOptions>()
                .AddSingleton<LyricsService>()
                .BuildServiceProvider())
            {
                serviceProvider.GetRequiredService<LyricsService>();
            }
        }

        /// <summary>
        ///     Tests the ability for dependency injection for <see cref="LyricsService"/>.
        /// </summary>
        [Fact]
        public void TestLyricsServiceDIWithCache()
        {
            using (var serviceProvider = new ServiceCollection()
                .AddSingleton<LyricsOptions>()
                .AddSingleton<LyricsService>()
                .AddSingleton<ILavalinkCache, LavalinkCache>()
                .BuildServiceProvider())
            {
                serviceProvider.GetRequiredService<LyricsService>();
            }
        }
    }
}
