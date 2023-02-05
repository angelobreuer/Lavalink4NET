namespace Lavalink4NET.Discord_NET.ExampleBot;

using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Rest.Entities.Tracks;

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
        var player = await GetPlayerAsync().ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        // when using StopAsync(true) the player also disconnects and clears the track queue.
        // DisconnectAsync only disconnects from the channel.
        await player.StopAsync(true).ConfigureAwait(false);
        await ReplyAsync("Disconnected.").ConfigureAwait(false);
    }

    /// <summary>
    ///     Plays music asynchronously.
    /// </summary>
    /// <param name="query">the search query</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("play", description: "Plays music", runMode: RunMode.Async)]
    public async Task Play(string query)
    {
        var player = await GetPlayerAsync(connectToVoiceChannel: true).ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        var track = await _audioService.Tracks
            .LoadTrackAsync(query, TrackSearchMode.YouTube)
            .ConfigureAwait(false);

        if (track is null)
        {
            await ReplyAsync("😖 No results.").ConfigureAwait(false);
            return;
        }

        await player.PlayAsync(track).ConfigureAwait(false);
        var position = 0; // TODO

        if (position is 0)
        {
            await ReplyAsync($"🔈 Playing: {track.Uri}").ConfigureAwait(false);
        }
        else
        {
            await ReplyAsync($"🔈 Added to queue: {track.Uri}").ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Shows the track position asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("position", description: "Shows the track position", runMode: RunMode.Async)]
    public async Task Position()
    {
        var player = await GetPlayerAsync().ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        if (player.CurrentTrack is null)
        {
            await ReplyAsync("Nothing playing!").ConfigureAwait(false);
            return;
        }

        await ReplyAsync($"Position: {player.Position} / {player.CurrentTrack.Duration}.").ConfigureAwait(false);
    }

    /// <summary>
    ///     Stops the current track asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("stop", description: "Stops the current track", runMode: RunMode.Async)]
    public async Task Stop()
    {
        var player = await GetPlayerAsync(connectToVoiceChannel: false);

        if (player is null)
        {
            return;
        }

        if (player.CurrentTrack is null)
        {
            await ReplyAsync("Nothing playing!").ConfigureAwait(false);
            return;
        }

        await player.StopAsync().ConfigureAwait(false);
        await ReplyAsync("Stopped playing.").ConfigureAwait(false);
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
            await ReplyAsync("Volume out of range: 0% - 1000%!").ConfigureAwait(false);
            return;
        }

        var player = await GetPlayerAsync().ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        await player.SetVolumeAsync(volume / 100f).ConfigureAwait(false);
        await ReplyAsync($"Volume updated: {volume}%").ConfigureAwait(false);
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
        var player = await _audioService.Players
            .GetPlayerAsync<VoteLavalinkPlayer>(Context.Guild.Id)
            .ConfigureAwait(false);

        if (player is not null && player.State is not PlayerState.Destroyed)
        {
            return player;
        }

        var user = Context.Guild.GetUser(Context.User.Id);

        if (!user.VoiceState.HasValue)
        {
            await ReplyAsync("You must be in a voice channel!").ConfigureAwait(false);
            return null;
        }

        if (!connectToVoiceChannel)
        {
            await ReplyAsync("The bot is not in a voice channel!").ConfigureAwait(false);
            return null;
        }

        return await _audioService.Players
            .JoinAsync(user.Guild.Id, user.VoiceChannel.Id, playerFactory: VoteLavalinkPlayer.Factory)
            .ConfigureAwait(false);
    }
}
