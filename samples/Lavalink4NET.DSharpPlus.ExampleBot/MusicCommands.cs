namespace ExampleBot;

using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.Options;

public class MusicCommands : ApplicationCommandModule
{
    private readonly IAudioService _audioService;

    public MusicCommands(IAudioService audioService)
    {
        ArgumentNullException.ThrowIfNull(audioService);

        _audioService = audioService;
    }

    [SlashCommand("play", description: "Plays music")]
    public async Task Play(InteractionContext interactionContext, [Option("query", "Track to play")] string query)
    {
        await interactionContext.DeferAsync().ConfigureAwait(false);

        var player = await GetPlayerAsync(interactionContext, connectToVoiceChannel: true).ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        var track = await _audioService.Tracks
            .LoadTrackAsync(query, TrackSearchMode.YouTube)
            .ConfigureAwait(false);

        if (track is null)
        {
            var errorResponse = new DiscordFollowupMessageBuilder()
                .WithContent("😖 No results.")
                .AsEphemeral();

            await interactionContext
                .FollowUpAsync(errorResponse)
                .ConfigureAwait(false);

            return;
        }

        var position = await player
            .PlayAsync(track)
            .ConfigureAwait(false);

        if (position is 0)
        {
            await interactionContext
                .FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent($"🔈 Playing: {track.Uri}"))
                .ConfigureAwait(false);
        }
        else
        {
            await interactionContext
                .FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent($"🔈 Added to queue: {track.Uri}"))
                .ConfigureAwait(false);
        }
    }

    private async ValueTask<QueuedLavalinkPlayer?> GetPlayerAsync(InteractionContext interactionContext, bool connectToVoiceChannel = true)
    {
        ArgumentNullException.ThrowIfNull(interactionContext);

        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

        var playerOptions = new QueuedLavalinkPlayerOptions { HistoryCapacity = 10000 };

        var result = await _audioService.Players
            .RetrieveAsync(interactionContext.Guild.Id, interactionContext.Member?.VoiceState.Channel.Id, playerFactory: PlayerFactory.Queued, Options.Create(playerOptions), retrieveOptions)
            .ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            var errorMessage = result.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
                PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
                _ => "Unknown error.",
            };

            var errorResponse = new DiscordFollowupMessageBuilder()
                .WithContent(errorMessage)
                .AsEphemeral();

            await interactionContext
                .FollowUpAsync(errorResponse)
                .ConfigureAwait(false);

            return null;
        }

        return result.Player;
    }
}