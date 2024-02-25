namespace ExampleBot;

using Lavalink4NET;
using Lavalink4NET.NetCord;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

public class MusicModule(IAudioService audioService) : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("play", "Plays a track!")]
    public async Task<string?> PlayAsync([SlashCommandParameter(Name = "query", Description = "The query to search for")] string query)
    {
        var player = await GetPlayerAsync(connectToVoiceChannel: true).ConfigureAwait(false);

        if (player is null)
        {
            return null; // error message is already sent by GetPlayerAsync
        }

        var track = await audioService.Tracks
            .LoadTrackAsync(query, TrackSearchMode.YouTube)
            .ConfigureAwait(false);

        if (track is null)
        {
            return "No tracks found.";
        }

        await player
            .PlayAsync(track)
            .ConfigureAwait(false);

        return $"Now playing: {track.Title}";
    }

    private async ValueTask<QueuedLavalinkPlayer?> GetPlayerAsync(bool connectToVoiceChannel = true)
    {
        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

        var result = await audioService.Players
            .RetrieveAsync(Context, playerFactory: PlayerFactory.Queued, retrieveOptions)
            .ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            var errorMessage = result.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
                PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
                _ => "Unknown error.",
            };

            await RespondAsync(InteractionCallback.Message(errorMessage)).ConfigureAwait(false);

            return null;
        }

        return result.Player;
    }
}