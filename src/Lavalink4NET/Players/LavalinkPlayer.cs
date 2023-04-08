namespace Lavalink4NET.Players;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Protocol;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Protocol.Requests;
using Lavalink4NET.Rest;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;

public class LavalinkPlayer : ILavalinkPlayer, ILavalinkPlayerListener
{
    private string? _currentTrackState;
    private bool _disposed;

    public LavalinkPlayer(IPlayerProperties<LavalinkPlayer, LavalinkPlayerOptions> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        SessionId = properties.SessionId;
        ApiClient = properties.ApiClient;
        GuildId = properties.InitialState.GuildId;
        VoiceChannelId = properties.VoiceChannelId;

        Refresh(properties.InitialState);
    }

    public LavalinkTrack? CurrentTrack { get; private set; }

    public ulong GuildId { get; }

    public bool IsPaused { get; private set; }

    public TimeSpan Position { get; }

    public PlayerState State => this switch
    {
        { _disposed: true, } => PlayerState.Destroyed,
        { IsPaused: true, } => PlayerState.Paused,
        { CurrentTrack: null, } => PlayerState.NotPlaying,
        _ => PlayerState.Playing,
    };

    public ulong VoiceChannelId { get; private set; }

    public float Volume { get; private set; }

    public ILavalinkApiClient ApiClient { get; }

    public string SessionId { get; }

    void ILavalinkPlayerListener.NotifyChannelUpdate(ulong voiceChannelId)
    {
        EnsureNotDestroyed();
        VoiceChannelId = voiceChannelId;
    }

    ValueTask ILavalinkPlayerListener.NotifyTrackEndedAsync(LavalinkTrack track, TrackEndReason endReason, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(track);
        return OnTrackEndedAsync(track, endReason, cancellationToken);
    }

    ValueTask ILavalinkPlayerListener.NotifyTrackExceptionAsync(LavalinkTrack track, TrackException exception, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(track);
        return OnTrackExceptionAsync(track, exception, cancellationToken);
    }

    ValueTask ILavalinkPlayerListener.NotifyTrackStartedAsync(LavalinkTrack track, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(track);
        return OnTrackStartedAsync(track, cancellationToken);
    }

    ValueTask ILavalinkPlayerListener.NotifyTrackStuckAsync(LavalinkTrack track, TimeSpan threshold, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(track);
        return OnTrackStuckAsync(track, threshold, cancellationToken);
    }

    public virtual async ValueTask PauseAsync(CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();

        var properties = new PlayerUpdateProperties { IsPaused = true, };
        await PerformUpdateAsync(properties, cancellationToken).ConfigureAwait(false);
    }

    public ValueTask PlayAsync(LavalinkTrack track, TrackPlayProperties properties = default, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();

        return PlayAsync(new TrackReference(track), properties, cancellationToken);
    }

    public ValueTask PlayAsync(string identifier, TrackPlayProperties properties = default, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();

        return PlayAsync(new TrackReference(identifier), properties, cancellationToken);
    }

    public virtual async ValueTask PlayAsync(TrackReference trackReference, TrackPlayProperties properties = default, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();

        var updateProperties = new PlayerUpdateProperties();

        if (trackReference.IsPresent)
        {
            var playableTrack = await trackReference.Track
                .GetPlayableTrackAsync(cancellationToken)
                .ConfigureAwait(false);

            updateProperties.TrackData = playableTrack.ToString();
        }
        else
        {
            updateProperties.Identifier = trackReference.Identifier;
        }

        if (properties.StartPosition is not null)
        {
            updateProperties.Position = properties.StartPosition.Value;
        }

        if (properties.EndTime is not null)
        {
            updateProperties.EndTime = properties.EndTime.Value;
        }

        await PerformUpdateAsync(updateProperties, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask RefreshAsync(CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();

        var model = await ApiClient
            .GetPlayerAsync(SessionId, GuildId, cancellationToken)
            .ConfigureAwait(false);

        Refresh(model!);
    }

    public virtual async ValueTask ResumeAsync(CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();

        var properties = new PlayerUpdateProperties { IsPaused = false, };
        await PerformUpdateAsync(properties, cancellationToken).ConfigureAwait(false);
    }

    public virtual ValueTask SeekAsync(TimeSpan position, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();

        throw new NotImplementedException();
    }

    public ValueTask SeekAsync(TimeSpan position, SeekOrigin seekOrigin, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();

        var targetPosition = seekOrigin switch
        {
            SeekOrigin.Begin => position,
            SeekOrigin.Current => Position + position,
            SeekOrigin.End => CurrentTrack.Duration + position,

            _ => throw new ArgumentOutOfRangeException(
                nameof(seekOrigin),
                seekOrigin,
                "Invalid seek origin."),
        };

        return SeekAsync(targetPosition, cancellationToken);
    }

    public virtual async ValueTask SetVolumeAsync(float volume, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var properties = new PlayerUpdateProperties { Volume = volume, };
        await PerformUpdateAsync(properties, cancellationToken).ConfigureAwait(false);
    }

    public virtual ValueTask StopAsync(bool disconnect, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();

        var properties = new PlayerUpdateProperties
        {
            TrackData = new Optional<string?>(null),
        };

        return PerformUpdateAsync(properties, cancellationToken);
    }

    protected void EnsureNotDestroyed()
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(LavalinkPlayer));
        }
#endif
    }

    protected virtual ValueTask OnTrackEndedAsync(LavalinkTrack track, TrackEndReason endReason, CancellationToken cancellationToken = default) => default;

    protected virtual ValueTask OnTrackExceptionAsync(LavalinkTrack track, TrackException exception, CancellationToken cancellationToken = default) => default;

    protected virtual ValueTask OnTrackStartedAsync(LavalinkTrack track, CancellationToken cancellationToken = default) => default;

    protected virtual ValueTask OnTrackStuckAsync(LavalinkTrack track, TimeSpan threshold, CancellationToken cancellationToken = default) => default;

    private async ValueTask PerformUpdateAsync(PlayerUpdateProperties properties, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(properties);

        var model = await ApiClient
            .UpdatePlayerAsync(SessionId, GuildId, properties, cancellationToken)
            .ConfigureAwait(false);

        Refresh(model!);
    }

    private void Refresh(PlayerInformationModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        Debug.Assert(model.GuildId == GuildId);

        IsPaused = model.IsPaused;

        if (_currentTrackState != model.CurrentTrack?.Data)
        {
            if (model.CurrentTrack is null)
            {
                _currentTrackState = null;
                CurrentTrack = null;
            }
            else
            {
                _currentTrackState = model.CurrentTrack.Data;

                var track = model.CurrentTrack.Information;

                CurrentTrack = new LavalinkTrack
                {
                    Author = track.Author,
                    Identifier = track.Identifier,
                    Title = track.Title,
                    Duration = track.Duration,
                    IsLiveStream = track.IsLiveStream,
                    IsSeekable = track.IsSeekable,
                    Uri = track.Uri,
                    SourceName = track.SourceName,
                    StartPosition = track.Position,
                };
            }
        }

        Volume = model.Volume;

        // TODO: restore filters
    }
}
