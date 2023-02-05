using System;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Discord_NET.ExampleBot;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using var serviceProvider = ConfigureServices();

var bot = serviceProvider.GetRequiredService<ExampleBot>();
var interactionService = serviceProvider.GetRequiredService<InteractionService>();

await bot.StartAsync();

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
    services.AddLavalink<DiscordClientWrapper>();

    services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Trace));

    services.AddSingleton(new LavalinkNodeOptions
    {
        // Your Node Configuration
    });

    return services.BuildServiceProvider();
}
