/*
 *  File:   ExampleBot.cs
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
