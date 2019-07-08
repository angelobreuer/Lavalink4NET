/*
 *  File:   Program.cs
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
    using System.Threading.Tasks;
    using Discord.Commands;
    using Discord.WebSocket;
    using Lavalink4NET.Logging;
    using Lavalink4NET.MemoryCache;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     Contains the main entry point.
    /// </summary>
    internal sealed class Program
    {
        /// <summary>
        ///     Starts the application.
        /// </summary>
        private static void Main()
            => RunAsync().GetAwaiter().GetResult();

        /// <summary>
        ///     Runs the bot asynchronously.
        /// </summary>
        private static async Task RunAsync()
        {
            using (var serviceProvider = ConfigureServices())
            {
                var bot = serviceProvider.GetRequiredService<ExampleBot>();
                var audio = serviceProvider.GetRequiredService<IAudioService>();
                var logger = serviceProvider.GetRequiredService<ILogger>() as EventLogger;
                logger.LogMessage += Log;

                await bot.StartAsync();
                await audio.InitializeAsync();

                logger.Log(bot, "Example Bot is running. Press [Q] to stop.");

                while (Console.ReadKey(true).Key != ConsoleKey.Q)
                {
                }

                await bot.StopAsync();
            }
        }

        private static void Log(object sender, LogMessageEventArgs args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"[{args.Source.GetType().Name} {args.Level}] ");

            Console.ResetColor();
            Console.WriteLine(args.Message);

            if (args.Exception != null)
            {
                Console.WriteLine(args.Exception);
            }
        }

        /// <summary>
        ///     Configures the application services.
        /// </summary>
        /// <returns>the service provider</returns>
        private static ServiceProvider ConfigureServices() => new ServiceCollection()
            // Bot
            .AddSingleton<ExampleBot>()

            // Discord
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandService>()

            // Lavalink
            .AddSingleton<IAudioService, LavalinkNode>()
            .AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>()
            .AddSingleton<ILogger, EventLogger>()

            .AddSingleton(new LavalinkNodeOptions
            {
                // Your Node Configuration
            })

            // Request Caching for Lavalink
            .AddSingleton<ILavalinkCache, LavalinkCache>()

            .BuildServiceProvider();
    }
}