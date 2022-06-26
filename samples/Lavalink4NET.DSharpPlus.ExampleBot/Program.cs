/*
 *  File:   Program.cs
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

using System;
using DSharpPlus;
using Lavalink4NET;
using Lavalink4NET.Cluster;
using Lavalink4NET.DSharpPlus;
using Lavalink4NET.MemoryCache;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using var provider = BuildServiceProvider();

var client = provider.GetRequiredService<DiscordClient>();
var audioService = provider.GetRequiredService<IAudioService>();

// connect to discord gateway and initialize node connection
await client.ConnectAsync();
await audioService.InitializeAsync();

// join channel
var track = await audioService.GetTrackAsync("<youtube search query>", SearchMode.YouTube);
var player = await audioService.JoinAsync<LavalinkPlayer>(BotCredentials.GuildId, BotCredentials.VoiceChannelId);

await player.PlayAsync(track);

// wait until user presses [Q]
while (Console.ReadKey(true).Key != ConsoleKey.Q)
{
}


static ServiceProvider BuildServiceProvider()
{
    var services = new ServiceCollection();

    // DSharpPlus
    services.AddSingleton<DiscordClient>();
    services.AddSingleton(new DiscordConfiguration { Token = BotCredentials.Token });

    // Lavalink
    services.AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>();
    services.AddSingleton<IAudioService, LavalinkCluster>();

    services.AddSingleton(new LavalinkClusterOptions
    {
        Nodes = new[]
        {
            new LavalinkNodeOptions
            {
                RestUri = BotCredentials.Node1.RestUri,
                Password = BotCredentials.Node1.Password,
                WebSocketUri = BotCredentials.Node1.WebSocketUri
                // add your node configuration
            },

            new LavalinkNodeOptions
            {
                RestUri = BotCredentials.Node2.RestUri,
                Password = BotCredentials.Node2.Password,
                WebSocketUri = BotCredentials.Node2.WebSocketUri
                // add your node configuration
            }
        },

        LoadBalacingStrategy = LoadBalancingStrategies.RoundRobinStrategy
    });

    // Request Caching for Lavalink
    services.AddSingleton<ILavalinkCache, LavalinkCache>();

    // Logging
    services.AddLogging(s => s.AddConsole().SetMinimumLevel(LogLevel.Trace));

    return services.BuildServiceProvider();
}
