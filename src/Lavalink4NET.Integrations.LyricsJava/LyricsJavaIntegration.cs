namespace Lavalink4NET.Integrations.LyricsJava;

using System.Collections.Immutable;
using Lavalink4NET.Events;
using Lavalink4NET.Integrations.LyricsJava.Events;
using Lavalink4NET.Integrations.LyricsJava.Extensions;
using Lavalink4NET.Integrations.LyricsJava.Models;
using Lavalink4NET.Integrations.LyricsJava.Players;
using Lavalink4NET.Protocol.Payloads;
using Lavalink4NET.Protocol.Payloads.Events;
using Microsoft.Extensions.Options;

public class LyricsJavaIntegration : ILavalinkIntegration, ILyricsJavaIntegration
{
    private readonly IAudioService _audioService;
    private readonly LyricsJavaIntegrationOptions _options;

    public LyricsJavaIntegration(IAudioService audioService, IOptions<LyricsJavaIntegrationOptions> options)
    {
        ArgumentNullException.ThrowIfNull(audioService);
        ArgumentNullException.ThrowIfNull(options);

        _audioService = audioService;
        _options = options.Value;
    }

    public event AsyncEventHandler<LyricsLoadedEventArgs>? LyricsLoaded;

    async ValueTask ILavalinkIntegration.ProcessPayloadAsync(IPayload payload, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(payload);

        if (_options.AutoResolve && payload is TrackStartEventPayload trackStartEventPayload)
        {
            var player = await _audioService.Players
                .GetPlayerAsync(trackStartEventPayload.GuildId, cancellationToken)
                .ConfigureAwait(false);

            if (player is null)
            {
                return;
            }

            var apiClient = await _audioService.ApiClientProvider
                .GetClientAsync(cancellationToken)
                .ConfigureAwait(false);

            var lyricsResult = await apiClient
                .GetCurrentTrackLyricsAsync(player.SessionId, player.GuildId, cancellationToken)
                .ConfigureAwait(false);

            var eventArgs = new LyricsLoadedEventArgs(
                guildId: trackStartEventPayload.GuildId,
                lyrics: lyricsResult);

            await LyricsLoaded
                .InvokeAsync(this, eventArgs)
                .ConfigureAwait(false);

            if (player is ILavaLyricsPlayerListener playerListener)
            {
                await playerListener
                    .NotifyLyricsLoadedAsync(lyricsResult, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }

    internal static LyricsTrack CreateLyricsTrack(LyricsResponseTrackModel model) => new(
        Title: model.Title,
        Author: model.Author,
        Album: model.Album,
        AlbumArt: model.AlbumArt.Select(x => new AlbumArt(x.Url, x.Width, x.Height)).ToImmutableArray());

    internal static Lyrics CreateLyrics(LyricsResponseModel? model) => new(
        Type: model?.Type == "timed" ? model.Type == "text" ? LyricsType.Basic : LyricsType.Timed : LyricsType.NotFound,
        Source: model?.Source,
        Basic: model?.LyricsText,
        Track: model?.Track is not null ? CreateLyricsTrack(model.Track) : null,
        Timed: model?.TimedLines?.Select(x => new TimedLyricsLine(x.Line, new TimeRange(x.Range.Start, x.Range.End))).ToImmutableArray());
}