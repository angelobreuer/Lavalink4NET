/* 
 *  File:   ExampleBot.cs
 *  Author: Angelo Breuer
 *  
 *  The MIT License (MIT)
 *  
 *  Copyright (c) Angelo Breuer 2019
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

namespace Lavalink4NET.Discord_NET.ExampleBot
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The main class for controlling the bot.
    /// </summary>
    public sealed class ExampleBot : IDisposable
    {
        private readonly DiscordSocketClient _client;
        private readonly ILogger<CommandService> _commandLogger;
        private readonly CommandService _commandService;
        private readonly ILogger<ExampleBot> _logger;
        private readonly IServiceProvider _provider;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExampleBot"/> class.
        /// </summary>
        /// <param name="provider">the service provider</param>
        public ExampleBot(IServiceProvider provider)
        {
            _client = provider.GetRequiredService<DiscordSocketClient>();
            _commandService = provider.GetRequiredService<CommandService>();
            _logger = provider.GetRequiredService<ILogger<ExampleBot>>();
            _commandLogger = provider.GetRequiredService<ILogger<CommandService>>();
            _client.MessageReceived += MessageReceived;
            _commandService.Log += Log;
            _provider = provider;
        }

        /// <summary>
        ///     Unregisters the events attached to the discord client.
        /// </summary>
        public void Dispose() => _client.MessageReceived -= MessageReceived;

        /// <summary>
        ///     Starts the bot asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        public async Task StartAsync()
        {
            await _client.LoginAsync(TokenType.Bot, "" /* Your Bot Token */);
            await _client.StartAsync();

            _logger.LogInformation($"Bot Identity: {_client.CurrentUser.Username}#{_client.CurrentUser.DiscriminatorValue}.");

            await _commandService.AddModulesAsync(GetType().Assembly, _provider);
        }

        /// <summary>
        ///     Stops the bot asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        public async Task StopAsync()
        {
            await _client.StopAsync();
        }

        private Task Log(LogMessage message)
        {
            if (message.Exception is CommandException commandException)
            {
                _commandLogger.LogWarning(commandException.GetBaseException(), $"{commandException.GetBaseException().GetType()} was " +
                    $"thrown while executing {commandException.Command.Aliases.First()} " +
                    $"in {commandException.Context.Channel} by {commandException.Context.User}.");
            }

            return Task.CompletedTask;
        }

        private async Task MessageReceived(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message) ||
                message.Source != MessageSource.User)
            {
                return;
            }

            var argPos = 0;

            if (!message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);
            await _commandService.ExecuteAsync(context, argPos, _provider);
        }
    }
}