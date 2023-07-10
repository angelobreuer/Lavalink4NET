using System.Collections.Immutable;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET.Cluster.Extensions;
using Lavalink4NET.Cluster.Nodes;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.DiscordNet.ExampleBot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = new HostApplicationBuilder(args);

// Discord
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddHostedService<DiscordClientHost>();

// Lavalink
builder.Services.AddLavalinkCluster<DiscordClientWrapper>();
builder.Services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Trace));

builder.Services.ConfigureLavalinkCluster(x =>
{
    x.Nodes = ImmutableArray.Create(
        new LavalinkClusterNodeOptions { BaseAddress = new Uri("http://localhost:2333/"), },
        new LavalinkClusterNodeOptions { BaseAddress = new Uri("http://localhost:2334/"), },
        new LavalinkClusterNodeOptions { BaseAddress = new Uri("http://localhost:2335/"), });
});

builder.Build().Run();
