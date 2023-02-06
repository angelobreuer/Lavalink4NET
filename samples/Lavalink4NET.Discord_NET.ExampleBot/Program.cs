using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Discord_NET.ExampleBot;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = new HostApplicationBuilder(args);

builder.Services.AddHostedService<ApplicationHost>();

// Bot
builder.Services.AddSingleton<ApplicationHost>();

// Discord
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton<InteractionService>();

// Lavalink
builder.Services.AddLavalink<DiscordClientWrapper>();
builder.Services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Trace));
builder.Services.AddSingleton(new LavalinkNodeOptions { /* Your Node Configuration */});

var app = builder.Build();
app.Run();