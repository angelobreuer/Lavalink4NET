using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET.DiscordNet.ExampleBot;
using Lavalink4NET.Extensions;
using Lavalink4NET.InactivityTracking.Extensions;
using Lavalink4NET.InactivityTracking.Trackers.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = new HostApplicationBuilder(args);

// Discord
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddHostedService<DiscordClientHost>();

// Lavalink
builder.Services.AddLavalink();
builder.Services.AddInactivityTracking();
builder.Services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Trace));

builder.Services.ConfigureInactivityTracking(x =>
{
});

builder.Services.Configure<UsersInactivityTrackerOptions>(options =>
{
    options.Threshold = 1;
    options.Timeout = TimeSpan.FromSeconds(30);
    options.ExcludeBots = true;
});

builder.Build().Run();
