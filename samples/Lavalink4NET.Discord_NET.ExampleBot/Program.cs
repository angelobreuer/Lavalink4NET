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
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Discord_NET.ExampleBot;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Logging.Microsoft;
using Lavalink4NET.MemoryCache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using var serviceProvider = ConfigureServices();

var bot = serviceProvider.GetRequiredService<ExampleBot>();
var interactionService = serviceProvider.GetRequiredService<InteractionService>();

var audioService = serviceProvider.GetRequiredService<IAudioService>();

await bot.StartAsync();
await audioService.InitializeAsync();

// Put your guild id to test on here:
await interactionService.RegisterCommandsToGuildAsync(894533462428635146);

while (Console.ReadKey(true).Key != ConsoleKey.Q)
{
}

await bot.StopAsync();

static ServiceProvider ConfigureServices()
{
    var services = new ServiceCollection();

    // Bot
    services.AddSingleton<ExampleBot>();

    // Discord
    services.AddSingleton<DiscordSocketClient>();
    services.AddSingleton<InteractionService>();

    // Lavalink
    services.AddSingleton<IAudioService, LavalinkNode>();
    services.AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>();
    services.AddMicrosoftExtensionsLavalinkLogging();

    services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Trace));

    services.AddSingleton(new LavalinkNodeOptions
    {
        // Your Node Configuration
    });

    // Request Caching for Lavalink
    services.AddSingleton<ILavalinkCache, LavalinkCache>();

    return services.BuildServiceProvider();
}
