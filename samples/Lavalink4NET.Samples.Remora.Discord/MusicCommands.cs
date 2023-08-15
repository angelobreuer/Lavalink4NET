namespace Lavalink4NET.Samples.Remora.Discord;

using System.ComponentModel;
using global::Remora.Commands.Attributes;
using global::Remora.Commands.Groups;
using global::Remora.Discord.Commands.Contexts;
using global::Remora.Discord.Commands.Extensions;
using global::Remora.Discord.Commands.Feedback.Messages;
using global::Remora.Discord.Commands.Feedback.Services;
using global::Remora.Results;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Remora.Discord;
using Lavalink4NET.Rest.Entities.Tracks;

public sealed class MusicCommands : CommandGroup
{
    private readonly ICommandContext _commandContext;
    private readonly IAudioService _audioService;
    private readonly FeedbackService _feedbackService;

    public MusicCommands(ICommandContext commandContext, IAudioService audioService, FeedbackService feedbackService)
    {
        ArgumentNullException.ThrowIfNull(commandContext);
        ArgumentNullException.ThrowIfNull(audioService);
        ArgumentNullException.ThrowIfNull(feedbackService);

        _commandContext = commandContext;
        _audioService = audioService;
        _feedbackService = feedbackService;
    }

    [Command("play")]
    [Description("Play a song.")]
    public async Task<IResult> PlayAsync([Description("Query")] string query)
    {
        var player = await GetPlayerAsync(connectToVoiceChannel: true).ConfigureAwait(false);

        if (player is null)
        {
            return Result.FromSuccess(); // error message is already sent
        }

        var track = await _audioService.Tracks
            .LoadTrackAsync(query, TrackSearchMode.YouTube)
            .ConfigureAwait(false);

        if (track is null)
        {
            await _feedbackService
                .SendContextualMessageAsync(new FeedbackMessage("No tracks found.", Colour: _feedbackService.Theme.FaultOrDanger))
                .ConfigureAwait(false);

            return Result.FromSuccess();
        }

        await player
            .PlayAsync(track)
            .ConfigureAwait(false);

        await _feedbackService
            .SendContextualMessageAsync(new FeedbackMessage($"Now playing: {track.Title}", Colour: _feedbackService.Theme.Success))
            .ConfigureAwait(false);

        return Result.FromSuccess();
    }

    private async ValueTask<QueuedLavalinkPlayer?> GetPlayerAsync(bool connectToVoiceChannel = true)
    {
        if (!_commandContext.TryGetGuildID(out var guildId))
        {
            return null;
        }

        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

        var result = await _audioService.Players
            .RetrieveAsync(_commandContext, playerFactory: PlayerFactory.Queued, retrieveOptions)
            .ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            var errorMessage = result.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
                PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
                _ => "Unknown error.",
            };

            await _feedbackService
                .SendContextualMessageAsync(new FeedbackMessage(errorMessage, Colour: _feedbackService.Theme.FaultOrDanger))
                .ConfigureAwait(false);

            return null;
        }

        return result.Player;
    }
}
