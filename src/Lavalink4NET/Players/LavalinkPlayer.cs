namespace Lavalink4NET.Players;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Protocol.Requests;
using Lavalink4NET.Rest;
using Lavalink4NET.Tracks;

public class LavalinkPlayer : ILavalinkPlayer, ILavalinkPlayerListener
{
    private readonly ILavalinkApiClient _apiClient;
    private readonly string _sessionId;
    private string? _currentTrackState;
    private bool _disposed;

    public LavalinkPlayer(PlayerProperties properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        _sessionId = properties.SessionId;
        _apiClient = properties.ApiClient;
        GuildId = properties.GuildId;
        VoiceChannelId = properties.VoiceChannelId;

        Refresh(properties.Model);
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

    void ILavalinkPlayerListener.NotifyChannelUpdate(ulong voiceChannelId)
    {
        EnsureNotDestroyed();
        VoiceChannelId = voiceChannelId;
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

    public ValueTask RefreshAsync(CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();

        throw new NotImplementedException();
    }

    public virtual ValueTask ResumeAsync(CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();

        throw new NotImplementedException();
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

    public virtual ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();
        return default;
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

    private async ValueTask PerformUpdateAsync(PlayerUpdateProperties properties, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(properties);

        var model = await _apiClient
            .UpdatePlayerAsync(_sessionId, GuildId, properties, cancellationToken)
            .ConfigureAwait(false);

        Refresh(model!);
    }

    private void Refresh(PlayerInformationModel model)
    {
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
