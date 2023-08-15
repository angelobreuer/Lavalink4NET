using Lavalink4NET.Extensions;
using Lavalink4NET.Remora.Discord;
using Lavalink4NET.Samples.Remora.Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Remora.Commands.Extensions;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Gateway;
using Remora.Discord.Hosting.Extensions;

var builder = new HostApplicationBuilder();

builder.Services.AddDiscordService(_ => "put bot token here");
builder.Services.Configure<DiscordGatewayClientOptions>(g => g.Intents |= GatewayIntents.MessageContents | GatewayIntents.GuildVoiceStates | GatewayIntents.Guilds);

builder.Services.AddDiscordCommands(true);

builder.Services.AddCommandTree().WithCommandGroup<MusicCommands>();

builder.Services.AddLavalink();

var app = builder.Build();

app.Run();