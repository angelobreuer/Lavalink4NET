namespace Lavalink4NET.Discord_NET.ExampleBot;

using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Lavalink4NET.Clients;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Lyrics;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;

/// <summary>
///     Presents some of the main features of the Lavalink4NET-Library.
/// </summary>
[RequireContext(ContextType.Guild)]
public sealed class MusicModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IAudioService _audioService;
    private readonly ILyricsService _lyricsService;

    public MusicModule(IAudioService audioService, ILyricsService lyricsService)
    {
        ArgumentNullException.ThrowIfNull(audioService);
        ArgumentNullException.ThrowIfNull(lyricsService);

        _audioService = audioService;
        _lyricsService = lyricsService;
    }

    [SlashCommand("lyrics", description: "Searches for lyrics", runMode: RunMode.Async)]
    public async Task Lyrics()
    {
        await DeferAsync().ConfigureAwait(false);

        var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        var track = player.CurrentTrack;

        if (track is null)
        {
            await FollowupAsync("🤔 No track is currently playing.").ConfigureAwait(false);
            return;
        }

        var lyrics = await _lyricsService.GetLyricsAsync(track.Title, track.Author).ConfigureAwait(false);

        if (lyrics is null)
        {
            await FollowupAsync("😖 No lyrics found.").ConfigureAwait(false);
            return;
        }

        await FollowupAsync($"📃 Lyrics for {track.Title} by {track.Author}:\n{lyrics}").ConfigureAwait(false);
    }

    [SlashCommand("play", description: "Plays music", runMode: RunMode.Async)]
    public async Task Play(string query)
    {
        await DeferAsync().ConfigureAwait(false);

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
            await FollowupAsync("😖 No results.").ConfigureAwait(false);
            return;
        }

        var position = await player.PlayAsync(track).ConfigureAwait(false);

        if (position is 0)
        {
            await FollowupAsync($"🔈 Playing: {track.Uri}").ConfigureAwait(false);
        }
        else
        {
            await FollowupAsync($"🔈 Added to queue: {track.Uri}").ConfigureAwait(false);
        }
    }

    private async ValueTask<QueuedLavalinkPlayer?> GetPlayerAsync(bool connectToVoiceChannel = true)
    {
        var joinOptions = new PlayerJoinOptions(ConnectToVoiceChannel: connectToVoiceChannel);

        var result = await _audioService.Players
            .GetOrJoinAsync(Context, playerFactory: PlayerFactory.Queued, joinOptions)
            .ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            var errorMessage = result.Status switch
            {
                PlayerJoinStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
                PlayerJoinStatus.BotNotConnected => "The bot is currently not connected.",
                _ => "Unknown error.",
            };

            await FollowupAsync(errorMessage).ConfigureAwait(false);
            return null;
        }

        return result.Player;
    }
}
