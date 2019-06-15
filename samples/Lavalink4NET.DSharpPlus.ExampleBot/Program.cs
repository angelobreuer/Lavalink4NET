/*
 *  File:   Program.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2019
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

namespace Lavalink4NET.DSharpPlus.ExampleBot
{
    using System;
    using System.Threading.Tasks;
    using global::DSharpPlus;
    using Lavalink4NET.Cluster;
    using Lavalink4NET.Player;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using static Microsoft.Extensions.Logging.LogLevel;

    internal class Program
    {
        private static void Main()
            => RunAsync().GetAwaiter().GetResult();

        private static ServiceProvider BuildServiceProvider() => new ServiceCollection()

            // DSharpPlus
            .AddSingleton<DiscordClient>()
            .AddSingleton(new DiscordConfiguration
            {
                Token = "" // TODO insert your bot token here
            })

            // Lavalink
            .AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>()
            .AddSingleton<IAudioService, LavalinkCluster>()

            .AddSingleton(new LavalinkClusterOptions
            {
                Nodes = new[]
                {
                    new LavalinkNodeOptions
                    {
                        // add your node configuration
                    },
                    new LavalinkNodeOptions
                    {
                        // add your node configuration
                    }
                },

                LoadBalacingStrategy = LoadBalacingStrategies.RoundRobinStrategy
            })

            // Logging
            .AddLogging(s => s.AddConsole().SetMinimumLevel(Trace))

            .BuildServiceProvider();

        private static async Task RunAsync()
        {
            using (var provider = BuildServiceProvider())
            {
                var client = provider.GetRequiredService<DiscordClient>();
                var audioService = provider.GetRequiredService<IAudioService>();
                var logger = provider.GetRequiredService<ILogger<Program>>();

                // connect to discord gateway and initialize node connection
                await client.ConnectAsync();
                await audioService.InitializeAsync();

                // join channel
                var track = await audioService.GetTrackAsync("<youtube search query>");
                var player = await audioService.JoinAsync<LavalinkPlayer>(/* Your Guild Id */0, /* Your Voice Channel Id */0);

                using (player)
                {
                    await player.PlayAsync(track);

                    logger.LogInformation("Ready. Press [Q] to exit.");

                    // wait until user presses [Q]
                    while (Console.ReadKey(true).Key != ConsoleKey.Q)
                    {
                    }
                }
            }
        }
    }
}