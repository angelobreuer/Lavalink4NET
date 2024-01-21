using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using Lavalink4NET;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = new HostApplicationBuilder(args);

// DSharpPlus
builder.Services.AddHostedService<ApplicationHost>();
builder.Services.AddSingleton<DiscordClient>();
builder.Services.AddSingleton(new DiscordConfiguration { Token = "", }); // Put token here

// Lavalink
builder.Services.AddLavalink();

// Logging
builder.Services.AddLogging(s => s.AddConsole().SetMinimumLevel(LogLevel.Trace));

builder.Build().Run();

file sealed class ApplicationHost : BackgroundService
{
    private readonly DiscordClient _discordClient;
    private readonly IAudioService _audioService;

    public ApplicationHost(DiscordClient discordClient, IAudioService audioService)
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(audioService);

        _discordClient = discordClient;
        _audioService = audioService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // connect to discord gateway and initialize node connection
        await _discordClient
            .ConnectAsync()
            .ConfigureAwait(false);

        await _audioService
            .WaitForReadyAsync(stoppingToken)
            .ConfigureAwait(false);

        var playerOptions = new LavalinkPlayerOptions
        {
            InitialTrack = new TrackReference("https://www.youtube.com/watch?v=dQw4w9WgXcQ"),
        };

        await _audioService.Players
            .JoinAsync(0, 0, playerOptions, stoppingToken) // Ids
            .ConfigureAwait(false);
    }
}