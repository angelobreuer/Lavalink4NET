namespace Lavalink4NET.Discord_NET.ExampleBot;

using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/// <summary>
///     The main class for controlling the bot.
/// </summary>
public sealed class ApplicationHost : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApplicationHost"/> class.
    /// </summary>
    /// <param name="serviceProvider">the service provider</param>
    public ApplicationHost(IServiceProvider serviceProvider)
    {
        _client = serviceProvider.GetRequiredService<DiscordSocketClient>();
        _interactionService = serviceProvider.GetRequiredService<InteractionService>();
        _serviceProvider = serviceProvider;

        _client.InteractionCreated += InteractionCreated;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _client.InteractionCreated -= InteractionCreated;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _client.LoginAsync(TokenType.Bot, "");
        await _client.StartAsync();

        await _interactionService.AddModulesAsync(GetType().Assembly, _serviceProvider);
        await Task.Delay(3000); // TODO
        // Put your guild id to test on here:
        await _interactionService.RegisterCommandsToGuildAsync(1023169934819328031);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _client.StopAsync();
    }

    private Task InteractionCreated(SocketInteraction interaction)
    {
        var ctx = new SocketInteractionContext(_client, interaction);
        return _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
    }
}
