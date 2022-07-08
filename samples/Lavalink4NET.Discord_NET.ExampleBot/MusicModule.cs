/*
 *  File:   MusicModule.cs
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
using Discord.Interactions;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;

/// <summary>
///     Presents some of the main features of the Lavalink4NET-Library.
/// </summary>
[RequireContext(ContextType.Guild)]
public sealed class MusicModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IAudioService _audioService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MusicModule"/> class.
    /// </summary>
    /// <param name="audioService">the audio service</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="audioService"/> is <see langword="null"/>.
    /// </exception>
    public MusicModule(IAudioService audioService)
        => _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));

    /// <summary>
    ///     Disconnects from the current voice channel connected to asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("disconnect", "Disconnects from the current voice channel connected to", runMode: RunMode.Async)]
    public async Task Disconnect()
    {
        var player = await GetPlayerAsync();

        if (player == null)
        {
            return;
        }

        // when using StopAsync(true) the player also disconnects and clears the track queue.
        // DisconnectAsync only disconnects from the channel.
        await player.StopAsync(true);
        await ReplyAsync("Disconnected.");
    }

    /// <summary>
    ///     Plays music asynchronously.
    /// </summary>
    /// <param name="query">the search query</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("play", description: "Plays music", runMode: RunMode.Async)]
    public async Task Play(string query)
    {
        var player = await GetPlayerAsync();

        if (player == null)
        {
            return;
        }

        var track = await _audioService.GetTrackAsync(query, SearchMode.YouTube);

        if (track == null)
        {
            await ReplyAsync("😖 No results.");
            return;
        }

        var position = await player.PlayAsync(track, enqueue: true);

        if (position == 0)
        {
            await ReplyAsync("🔈 Playing: " + track.Source);
        }
        else
        {
            await ReplyAsync("🔈 Added to queue: " + track.Source);
        }
    }

    /// <summary>
    ///     Shows the track position asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("position", description: "Shows the track position", runMode: RunMode.Async)]
    public async Task Position()
    {
        var player = await GetPlayerAsync();

        if (player == null)
        {
            return;
        }

        if (player.CurrentTrack == null)
        {
            await ReplyAsync("Nothing playing!");
            return;
        }

        await ReplyAsync($"Position: {player.Position.Position} / {player.CurrentTrack.Duration}.");
    }

    /// <summary>
    ///     Stops the current track asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("stop", description: "Stops the current track", runMode: RunMode.Async)]
    public async Task Stop()
    {
        var player = await GetPlayerAsync(connectToVoiceChannel: false);

        if (player == null)
        {
            return;
        }

        if (player.CurrentTrack == null)
        {
            await ReplyAsync("Nothing playing!");
            return;
        }

        await player.StopAsync();
        await ReplyAsync("Stopped playing.");
    }

    /// <summary>
    ///     Updates the player volume asynchronously.
    /// </summary>
    /// <param name="volume">the volume (1 - 1000)</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("volume", description: "Sets the player volume (0 - 1000%)", runMode: RunMode.Async)]
    public async Task Volume(int volume = 100)
    {
        if (volume is > 1000 or < 0)
        {
            await ReplyAsync("Volume out of range: 0% - 1000%!");
            return;
        }

        var player = await GetPlayerAsync();

        if (player == null)
        {
            return;
        }

        await player.SetVolumeAsync(volume / 100f);
        await ReplyAsync($"Volume updated: {volume}%");
    }

    /// <summary>
    ///     Gets the guild player asynchronously.
    /// </summary>
    /// <param name="connectToVoiceChannel">
    ///     a value indicating whether to connect to a voice channel
    /// </param>
    /// <returns>
    ///     a task that represents the asynchronous operation. The task result is the lavalink player.
    /// </returns>
    private async ValueTask<VoteLavalinkPlayer> GetPlayerAsync(bool connectToVoiceChannel = true)
    {
        var player = _audioService.GetPlayer<VoteLavalinkPlayer>(Context.Guild.Id);

        if (player != null
            && player.State != PlayerState.NotConnected
            && player.State != PlayerState.Destroyed)
        {
            return player;
        }

        var user = Context.Guild.GetUser(Context.User.Id);

        if (!user.VoiceState.HasValue)
        {
            await ReplyAsync("You must be in a voice channel!");
            return null;
        }

        if (!connectToVoiceChannel)
        {
            await ReplyAsync("The bot is not in a voice channel!");
            return null;
        }

        return await _audioService.JoinAsync<VoteLavalinkPlayer>(user.Guild.Id, user.VoiceChannel.Id);
    }
}
