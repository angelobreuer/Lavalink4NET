/*
 *  File:   MusicModule.cs
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
    using System.Threading.Tasks;
    using Discord.Commands;
    using Lavalink4NET.Player;
    using Lavalink4NET.Rest;

    /// <summary>
    ///     Presents some of the main features of the Lavalink4NET-Library.
    /// </summary>
    [Name("Music")]
    [RequireContext(ContextType.Guild)]
    public sealed class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly IAudioService _audioService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MusicModule"/> class.
        /// </summary>
        /// <param name="audioService">the audio service</param>
        public MusicModule(IAudioService audioService) => _audioService = audioService;

        private async Task<VoteLavalinkPlayer> GetPlayerAsync()
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

            return await _audioService.JoinAsync<VoteLavalinkPlayer>(Context.Guild.Id, user.VoiceChannel.Id);
        }

        /// <summary>
        ///     Plays music from YouTube asynchronously.
        /// </summary>
        /// <param name="query">the search query</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        [Command("play")]
        public async Task Play([Remainder]string query)
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

        [Command("stop")]
        public async Task Stop()
        {
            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

            await player.StopAsync();
        }

        [Command("disconnect")]
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
        }

        [Command("volume")]
        public async Task Volume(int volume = 100)
        {
            if (volume > 1000 || volume < 0)
            {
                await ReplyAsync("Volume out of range: 0 - 1000!");
                return;
            }

            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

            await player.SetVolumeAsync(volume / 100f);
        }

        [Command("position")]
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

            await ReplyAsync($"Position: {player.TrackPosition} / {player.CurrentTrack.Duration}.");
        }
    }
}