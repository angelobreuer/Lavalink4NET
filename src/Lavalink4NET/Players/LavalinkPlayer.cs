namespace Lavalink4NET.Players;

using System;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Protocol;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Protocol.Models.Filters;
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
    private readonly bool _disconnectOnStop;
    private readonly IPlayerLifecycle _playerLifecycle;
    private string? _currentTrackState;
    private int _disposed;
    private DateTimeOffset _syncedAt;
    private TimeSpan _unstretchedRelativePosition;
    private bool _disconnectOnDestroy;
    private bool _connectedOnce;

    public LavalinkPlayer(IPlayerProperties<LavalinkPlayer, LavalinkPlayerOptions> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        SessionId = properties.SessionId;
        ApiClient = properties.ApiClient;
        DiscordClient = properties.DiscordClient;
        GuildId = properties.InitialState.GuildId;
        VoiceChannelId = properties.VoiceChannelId;

        Label = properties.Label;
        _systemClock = properties.SystemClock;
        _logger = properties.Logger;
        _syncedAt = properties.SystemClock.UtcNow;
        _playerLifecycle = properties.Lifecycle;

        _unstretchedRelativePosition = default;
        _connectedOnce = false;

        _disconnectOnDestroy = properties.Options.Value.DisconnectOnDestroy;
        _disconnectOnStop = properties.Options.Value.DisconnectOnStop;

        VoiceServer = new VoiceServer(properties.InitialState.VoiceState.Token, properties.InitialState.VoiceState.Endpoint);
        VoiceState = new VoiceState(properties.VoiceChannelId, properties.InitialState.VoiceState.SessionId);

        Filters = new PlayerFilterMap(this);

        if (properties.InitialState.IsPaused)
        {
            State = PlayerState.Paused;
        }
        else if (properties.InitialState.CurrentTrack is null)
        {
            State = PlayerState.NotPlaying;
        }
        else
        {
            State = PlayerState.Playing;
        }

        Refresh(properties.InitialState);
    }

    public LavalinkTrack? CurrentTrack { get; private set; }

    public ulong GuildId { get; }

    public bool IsPaused { get; private set; }

    public VoiceServer? VoiceServer { get; private set; }

    public VoiceState VoiceState { get; private set; }

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

    public PlayerState State { get; private set; }

    public ulong VoiceChannelId { get; private set; }

    public float Volume { get; private set; }

    public ILavalinkApiClient ApiClient { get; }

    public string SessionId { get; }

    public PlayerConnectionState ConnectionState { get; private set; }

    public IDiscordClientWrapper DiscordClient { get; }

    public IPlayerFilters Filters { get; }

    public string Label { get; }

    async ValueTask ILavalinkPlayerListener.NotifyChannelUpdateAsync(ulong? voiceChannelId, CancellationToken cancellationToken)
    {
        if (voiceChannelId is null)
        {
            _logger.PlayerDisconnected(Label);
            await using var _ = this.ConfigureAwait(false);
            return;
        }

        EnsureNotDestroyed();

        if (!_connectedOnce)
        {
            _connectedOnce = true;
            _logger.PlayerConnected(Label, voiceChannelId);
        }
        else
        {
            _logger.PlayerMoved(Label, voiceChannelId);
        }

        VoiceChannelId = voiceChannelId.Value;

        try
        {
            await NotifyChannelUpdateAsync(voiceChannelId, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (_disconnectOnDestroy && voiceChannelId is null)
            {
                await DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    async ValueTask ILavalinkPlayerListener.NotifyTrackEndedAsync(LavalinkTrack track, TrackEndReason endReason, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(track);

        CurrentTrack = null;

        await UpdateStateAsync(PlayerState.NotPlaying, cancellationToken)
            .ConfigureAwait(false);

        await NotifyTrackEndedAsync(track, endReason, cancellationToken).ConfigureAwait(false);
    }

    ValueTask ILavalinkPlayerListener.NotifyTrackExceptionAsync(LavalinkTrack track, TrackException exception, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(track);
        return NotifyTrackExceptionAsync(track, exception, cancellationToken);
    }

    ValueTask ILavalinkPlayerListener.NotifyTrackStartedAsync(LavalinkTrack track, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(track);
        CurrentTrack = track;
        return NotifyTrackStartedAsync(track, cancellationToken);
    }

    ValueTask ILavalinkPlayerListener.NotifyTrackStuckAsync(LavalinkTrack track, TimeSpan threshold, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(track);
        return NotifyTrackStuckAsync(track, threshold, cancellationToken);
    }

    public virtual async ValueTask PauseAsync(CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();

        var properties = new PlayerUpdateProperties { IsPaused = true, };
        await PerformUpdateAsync(properties, cancellationToken).ConfigureAwait(false);

        _logger.PlayerPaused(Label);
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

        _logger.PlayerResumed(Label);
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

        _logger.PlayerVolumeChanged(Label, volume);
    }

    public virtual async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();

        var properties = new PlayerUpdateProperties
        {
            TrackData = new Optional<string?>(null),
        };

        await PerformUpdateAsync(properties, cancellationToken).ConfigureAwait(false);

        _logger.PlayerStopped(Label);

        if (_disconnectOnStop)
        {
            await DisposeAsync().ConfigureAwait(false);
        }
    }

    async ValueTask ILavalinkPlayerListener.NotifyWebSocketClosedAsync(WebSocketCloseStatus closeStatus, string reason, bool byRemote, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        VoiceServer = null;
        VoiceState = new VoiceState(VoiceState.VoiceChannelId, SessionId: null);

        await NotifyWebSocketClosedAsync(closeStatus, reason, byRemote, cancellationToken).ConfigureAwait(false);
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

    protected virtual ValueTask NotifyWebSocketClosedAsync(WebSocketCloseStatus closeStatus, string reason, bool byRemote = false, CancellationToken cancellationToken = default) => default;

    protected virtual ValueTask NotifyTrackEndedAsync(LavalinkTrack track, TrackEndReason endReason, CancellationToken cancellationToken = default) => default;

    protected virtual ValueTask NotifyChannelUpdateAsync(ulong? voiceChannelId, CancellationToken cancellationToken = default) => default;

    protected virtual ValueTask NotifyTrackExceptionAsync(LavalinkTrack track, TrackException exception, CancellationToken cancellationToken = default) => default;

    protected virtual ValueTask NotifyTrackStartedAsync(LavalinkTrack track, CancellationToken cancellationToken = default) => default;

    protected virtual ValueTask NotifyTrackStuckAsync(LavalinkTrack track, TimeSpan threshold, CancellationToken cancellationToken = default) => default;

    protected virtual ValueTask NotifyFiltersUpdatedAsync(IPlayerFilters filters, CancellationToken cancellationToken = default) => default;

    private async ValueTask PerformUpdateAsync(PlayerUpdateProperties properties, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(properties);

        var model = await ApiClient
            .UpdatePlayerAsync(SessionId, GuildId, properties, cancellationToken)
            .ConfigureAwait(false);

        Refresh(model!);

        var state = this switch
        {
            { IsPaused: true, } => PlayerState.Paused,
            { CurrentTrack: null, } => PlayerState.NotPlaying,
            _ => PlayerState.Playing,
        };

        await UpdateStateAsync(state, cancellationToken).ConfigureAwait(false);
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
                    ArtworkUri = track.ArtworkUri,
                    Isrc = track.Isrc,
                    TrackData = model.CurrentTrack.Data,
                    AdditionalInformation = model.CurrentTrack.AdditionalInformation,
                };
            }
        }

        Volume = model.Volume;

        // TODO: restore filters
    }

    internal async ValueTask UpdateFiltersAsync(PlayerFilterMapModel filterMap, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var properties = new PlayerUpdateProperties
        {
            Filters = filterMap,
        };

        _logger.PlayerFiltersChanged(Label);

        await PerformUpdateAsync(properties, cancellationToken).ConfigureAwait(false);
        await NotifyFiltersUpdatedAsync(Filters, cancellationToken).ConfigureAwait(false);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) is not 0)
        {
            return;
        }

        await UpdateStateAsync(PlayerState.Destroyed).ConfigureAwait(false);

        // Dispose the lifecycle to notify the player is being destroyed
        await using var _ = _playerLifecycle.ConfigureAwait(false);

        _logger.PlayerDestroyed(Label);

        await ApiClient
            .DestroyPlayerAsync(SessionId, GuildId)
            .ConfigureAwait(false);

        if (_disconnectOnDestroy)
        {
            await DiscordClient
                .SendVoiceUpdateAsync(GuildId, null, false, false)
                .ConfigureAwait(false);
        }
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

        _logger.PlayerUpdateProcessed(Label, timestamp, position, connected, latency);

        return default;
    }

    public async ValueTask DisconnectAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await using var _ = this.ConfigureAwait(false);

        await DiscordClient
            .SendVoiceUpdateAsync(GuildId, null, false, false, cancellationToken)
            .ConfigureAwait(false);

        _disconnectOnDestroy = false;
    }

    private async ValueTask UpdateStateAsync(PlayerState playerState, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

#if DEBUG
        var computedState = this switch
        {
            { _disposed: not 0, } => PlayerState.Destroyed,
            { IsPaused: true, } => PlayerState.Paused,
            { CurrentTrack: null, } => PlayerState.NotPlaying,
            _ => PlayerState.Playing,
        };

        Debug.Assert(playerState == computedState, $"playerState ({playerState}) == computedState ({computedState})");
#endif

        if (playerState == State)
        {
            return;
        }

        State = playerState;

        await _playerLifecycle
            .NotifyStateChangedAsync(playerState, cancellationToken)
            .ConfigureAwait(false);
    }

    protected virtual ValueTask NotifyVoiceStateUpdatedAsync(VoiceState voiceState, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        VoiceState = voiceState;

        if (voiceState.VoiceChannelId is null)
        {
            return ValueTask.CompletedTask;
        }

        return UpdateVoiceCredentialsAsync(cancellationToken);
    }

    protected virtual ValueTask NotifyVoiceServerUpdatedAsync(VoiceServer voiceServer, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        VoiceServer = voiceServer;
        return UpdateVoiceCredentialsAsync(cancellationToken);
    }

    ValueTask ILavalinkPlayerListener.NotifyVoiceStateUpdatedAsync(VoiceState voiceState, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return NotifyVoiceStateUpdatedAsync(voiceState, cancellationToken);
    }

    ValueTask ILavalinkPlayerListener.NotifyVoiceServerUpdatedAsync(VoiceServer voiceServer, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return NotifyVoiceServerUpdatedAsync(voiceServer, cancellationToken);
    }

    private ValueTask UpdateVoiceCredentialsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (VoiceServer is null || VoiceState.SessionId is null)
        {
            return ValueTask.CompletedTask;
        }

        var properties = new PlayerUpdateProperties
        {
            VoiceState = new VoiceStateProperties(
                Token: VoiceServer.Value.Token,
                Endpoint: VoiceServer.Value.Endpoint,
                SessionId: VoiceState.SessionId),
        };

        return PerformUpdateAsync(properties, cancellationToken);
    }
}

internal static partial class Logging
{
    [LoggerMessage(1, LogLevel.Trace, "[{Label}] Processed player update (absolute timestamp: {AbsoluteTimestamp}, relative track position: {Position}, connected: {IsConnected}, latency: {Latency}).", EventName = nameof(PlayerUpdateProcessed))]
    public static partial void PlayerUpdateProcessed(this ILogger<LavalinkPlayer> logger, string label, DateTimeOffset absoluteTimestamp, TimeSpan position, bool isConnected, TimeSpan? latency);

    [LoggerMessage(2, LogLevel.Information, "[{Label}] Player moved to channel {ChannelId}.", EventName = nameof(PlayerMoved))]
    public static partial void PlayerMoved(this ILogger<LavalinkPlayer> logger, string label, ulong? channelId);

    [LoggerMessage(3, LogLevel.Information, "[{Label}] Player connected to channel {ChannelId}.", EventName = nameof(PlayerMoved))]
    public static partial void PlayerConnected(this ILogger<LavalinkPlayer> logger, string label, ulong? channelId);

    [LoggerMessage(4, LogLevel.Information, "[{Label}] Player disconnected from channel.", EventName = nameof(PlayerDisconnected))]
    public static partial void PlayerDisconnected(this ILogger<LavalinkPlayer> logger, string label);

    [LoggerMessage(5, LogLevel.Information, "[{Label}] Player paused.", EventName = nameof(PlayerPaused))]
    public static partial void PlayerPaused(this ILogger<LavalinkPlayer> logger, string label);

    [LoggerMessage(6, LogLevel.Information, "[{Label}] Player resumed.", EventName = nameof(PlayerResumed))]
    public static partial void PlayerResumed(this ILogger<LavalinkPlayer> logger, string label);

    [LoggerMessage(7, LogLevel.Information, "[{Label}] Player stopped.", EventName = nameof(PlayerStopped))]
    public static partial void PlayerStopped(this ILogger<LavalinkPlayer> logger, string label);

    [LoggerMessage(8, LogLevel.Information, "[{Label}] Player volume changed to {Volume}.", EventName = nameof(PlayerVolumeChanged))]
    public static partial void PlayerVolumeChanged(this ILogger<LavalinkPlayer> logger, string label, float volume);

    [LoggerMessage(9, LogLevel.Information, "[{Label}] Player filters changed.", EventName = nameof(PlayerFiltersChanged))]
    public static partial void PlayerFiltersChanged(this ILogger<LavalinkPlayer> logger, string label);

    [LoggerMessage(10, LogLevel.Information, "[{Label}] Player destroyed.", EventName = nameof(PlayerDestroyed))]
    public static partial void PlayerDestroyed(this ILogger<LavalinkPlayer> logger, string label);
}