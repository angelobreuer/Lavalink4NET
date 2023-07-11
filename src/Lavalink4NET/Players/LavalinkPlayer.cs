namespace Lavalink4NET.Players;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Protocol;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Protocol.Requests;
using Lavalink4NET.Rest;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

public class LavalinkPlayer : ILavalinkPlayer, ILavalinkPlayerListener
{
    private readonly ILogger<LavalinkPlayer> _logger;
    private readonly ISystemClock _systemClock;
    private string? _currentTrackState;
    private int _disposed;
    private DateTimeOffset _syncedAt;
    private TimeSpan _unstretchedRelativePosition;

    public LavalinkPlayer(IPlayerProperties<LavalinkPlayer, LavalinkPlayerOptions> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        SessionId = properties.SessionId;
        ApiClient = properties.ApiClient;
        DiscordClient = properties.DiscordClient;
        GuildId = properties.InitialState.GuildId;
        VoiceChannelId = properties.VoiceChannelId;

        _systemClock = properties.SystemClock;
        _logger = properties.Logger;
        _syncedAt = properties.SystemClock.UtcNow;
        _unstretchedRelativePosition = default;

        Refresh(properties.InitialState);
    }

    public LavalinkTrack? CurrentTrack { get; private set; }

    public ulong GuildId { get; }

    public bool IsPaused { get; private set; }

    public TrackPosition? Position
    {
        get
        {
            if (CurrentTrack is null)
            {
                return null;
            }

            return new TrackPosition(
                SystemClock: _systemClock,
                SyncedAt: _syncedAt,
                UnstretchedRelativePosition: _unstretchedRelativePosition,
                TimeStretchFactor: 1F); // TODO: time stretch
        }
    }

    public PlayerState State => this switch
    {
        { _disposed: not 0, } => PlayerState.Destroyed,
        { IsPaused: true, } => PlayerState.Paused,
        { CurrentTrack: null, } => PlayerState.NotPlaying,
        _ => PlayerState.Playing,
    };

    public ulong VoiceChannelId { get; private set; }

    public float Volume { get; private set; }

    public ILavalinkApiClient ApiClient { get; }

    public string SessionId { get; }

    public PlayerConnectionState ConnectionState { get; private set; }

    public IDiscordClientWrapper DiscordClient { get; }

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

    public virtual async ValueTask SeekAsync(TimeSpan position, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();

        var properties = new PlayerUpdateProperties { Position = position, };
        await PerformUpdateAsync(properties, cancellationToken).ConfigureAwait(false);
    }

    public ValueTask SeekAsync(TimeSpan position, SeekOrigin seekOrigin, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();

        var targetPosition = seekOrigin switch
        {
            SeekOrigin.Begin => position,
            SeekOrigin.Current => Position!.Value.Position + position, // TODO: check how this works with time stretch
            SeekOrigin.End => CurrentTrack!.Duration + position,

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

    public virtual async ValueTask StopAsync(bool disconnect = false, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();

        var properties = new PlayerUpdateProperties
        {
            TrackData = new Optional<string?>(null),
        };

        if (disconnect)
        {
            await DiscordClient
                .SendVoiceUpdateAsync(GuildId, null, false, false, cancellationToken)
                .ConfigureAwait(false);
        }

        await PerformUpdateAsync(properties, cancellationToken).ConfigureAwait(false);
    }

    protected void EnsureNotDestroyed()
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed is not 0, this);
#else
        if (_disposed is not 0)
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

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) is 0)
        {
            return;
        }

        await ApiClient
            .DestroyPlayerAsync(SessionId, GuildId)
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public ValueTask NotifyPlayerUpdateAsync(
        DateTimeOffset timestamp,
        TimeSpan position,
        bool connected,
        TimeSpan? latency,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _unstretchedRelativePosition = position;
        _syncedAt = timestamp;

        ConnectionState = new PlayerConnectionState(
            IsConnected: connected,
            Latency: latency);

        _logger.LogTrace(
            "[{PlayerId}] Processed player update (absolute timestamp: {AbsoluteTimestamp}, relative track position: {Position}, connected: {IsConnected}, latency: {Latency}).",
            GuildId, timestamp, position, connected, latency);

        return default;
    }
}
