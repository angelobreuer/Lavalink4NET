namespace Lavalink4NET.Discord_NET.ExampleBot;

using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
///     The main class for controlling the bot.
/// </summary>
public sealed class ExampleBot : IDisposable
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExampleBot"/> class.
    /// </summary>
    /// <param name="serviceProvider">the service provider</param>
    public ExampleBot(IServiceProvider serviceProvider)
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

    /// <summary>
    ///     Starts the bot asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    public async Task StartAsync()
    {
        // Insert your bot token here:
        await _client.LoginAsync(TokenType.Bot, "");
        await _client.StartAsync();

        await _interactionService.AddModulesAsync(GetType().Assembly, _serviceProvider);
        await Task.Delay(3000); // TODO
        // Put your guild id to test on here:
        await _interactionService.RegisterCommandsToGuildAsync(1023169934819328031);
    }

    /// <summary>
    ///     Stops the bot asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    public async Task StopAsync()
    {
        await _client.StopAsync();
    }

    private Task InteractionCreated(SocketInteraction interaction)
    {
        var ctx = new SocketInteractionContext(_client, interaction);
        return _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
    }
}
