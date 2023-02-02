using System;
using DSharpPlus;
using Lavalink4NET;
using Lavalink4NET.DSharpPlus;
using Lavalink4NET.Extensions;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using var provider = BuildServiceProvider();

var client = provider.GetRequiredService<DiscordClient>();
var audioService = provider.GetRequiredService<IAudioService>();

// connect to discord gateway and initialize node connection
await client
    .ConnectAsync()
    .ConfigureAwait(false);

// join channel
var track = await audioService.Tracks
    .LoadTrackAsync("<youtube search query>", TrackSearchMode.YouTube)
    .ConfigureAwait(false);

var player = await audioService.Players
    .JoinAsync(0, 0) // Ids
    .ConfigureAwait(false);

await player
    .PlayAsync(track)
    .ConfigureAwait(false);

// wait until user presses [Q]
while (Console.ReadKey(true).Key != ConsoleKey.Q)
{
}


static ServiceProvider BuildServiceProvider()
{
    var services = new ServiceCollection();

    // DSharpPlus
    services.AddSingleton<DiscordClient>();
    services.AddSingleton(new DiscordConfiguration { Token = "", }); // Put token here

    // Lavalink
    services.AddLavalink<DiscordClientWrapper>();

    // Logging
    services.AddLogging(s => s.AddConsole().SetMinimumLevel(LogLevel.Trace));

    return services.BuildServiceProvider();
}
