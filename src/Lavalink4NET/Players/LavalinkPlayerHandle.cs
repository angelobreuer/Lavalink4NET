namespace Lavalink4NET.Players;

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Protocol.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class LavalinkPlayerHandle<TPlayer, TOptions> : ILavalinkPlayerHandle
    where TPlayer : ILavalinkPlayer
    where TOptions : LavalinkPlayerOptions
{
    private readonly ulong _guildId;
    private readonly ILogger<TPlayer> _logger;
    private readonly IOptions<TOptions> _options;
    private readonly PlayerContext _playerContext;
    private readonly PlayerFactory<TPlayer, TOptions> _playerFactory;
    private string? _sessionId;
    private object _value;
    private VoiceServer? _voiceServer;
    private VoiceState? _voiceState;
    public LavalinkPlayerHandle(
        ulong guildId,
        PlayerContext playerContext,
        PlayerFactory<TPlayer, TOptions> playerFactory,
        IOptions<TOptions> options,
        ILogger<TPlayer> logger)
    {
        ArgumentNullException.ThrowIfNull(playerContext);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _value = new TaskCompletionSource<ILavalinkPlayer>(TaskCreationOptions.RunContinuationsAsynchronously);

        _guildId = guildId;
        _playerContext = playerContext;
        _playerFactory = playerFactory;
        _options = options;
        _logger = logger;
    }

    public ILavalinkPlayer? Player => _value as ILavalinkPlayer;

    public ValueTask<ILavalinkPlayer> GetPlayerAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_value is TaskCompletionSource<ILavalinkPlayer> taskCompletionSource)
        {
            return new ValueTask<ILavalinkPlayer>(task: taskCompletionSource.Task);
        }

        return ValueTask.FromResult<ILavalinkPlayer>(Unsafe.As<object, TPlayer>(ref Unsafe.AsRef(_value)));
    }

    public async ValueTask AssociateAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(sessionId);

        _sessionId = sessionId;

        if (_voiceServer is not null && _voiceState is not null)
        {
            await CompleteAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async ValueTask UpdateVoiceServerAsync(VoiceServer voiceServer, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(voiceServer);

        _voiceServer = voiceServer;

        if (_voiceState is not null && _sessionId is not null)
        {
            await CompleteAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async ValueTask UpdateVoiceServerAsync(VoiceState voiceState, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(voiceState);

        _voiceState = voiceState;

        if (_voiceServer is not null && _sessionId is not null)
        {
            await CompleteAsync(cancellationToken).ConfigureAwait(false);
        }
    }
    private async ValueTask CompleteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Debug.Assert(_voiceServer is not null);
        Debug.Assert(_voiceState is not null);
        Debug.Assert(_sessionId is not null);

        if (_value is TaskCompletionSource<ILavalinkPlayer> taskCompletionSource)
        {
            var player = await CreatePlayerAsync(cancellationToken).ConfigureAwait(false);
            _value = player;

            taskCompletionSource.TrySetResult(player);
        }

        if (_value is ILavalinkPlayerListener playerListener)
        {
            playerListener.NotifyChannelUpdate(_voiceState.Value.VoiceChannelId!.Value);
        }
    }

    private async ValueTask<TPlayer> CreatePlayerAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Debug.Assert(_voiceServer is not null);
        Debug.Assert(_voiceState is not null);
        Debug.Assert(_sessionId is not null);

        var playerProperties = new PlayerUpdateProperties
        {
            VoiceState = new VoiceStateProperties(
                Token: _voiceServer.Value.Token,
                Endpoint: _voiceServer.Value.Endpoint,
                SessionId: _voiceState.Value.SessionId),
        };

        if (_options.Value.InitialTrack is not null)
        {
            var track = _options.Value.InitialTrack.Value;

            playerProperties = track.IsPresent
                ? (playerProperties with { TrackData = track.Track.ToString(), })
                : (playerProperties with { Identifier = track.Identifier, });
        }

        if (_options.Value.InitialVolume is not null)
        {
            playerProperties = playerProperties with { Volume = _options.Value.InitialVolume.Value, };
        }

        var initialState = await _playerContext.ApiClient
            .UpdatePlayerAsync(_sessionId, _guildId, playerProperties, cancellationToken)
            .ConfigureAwait(false);

        var label = _options.Value.Label ?? $"{typeof(TPlayer)}@{_guildId}";

        var properties = new PlayerProperties<TPlayer, TOptions>(
            Context: _playerContext,
            VoiceChannelId: _voiceState.Value.VoiceChannelId!.Value,
            InitialState: initialState,
            Label: label,
            SessionId: _sessionId,
            Options: _options,
            Logger: _logger);

        return await _playerFactory(properties, cancellationToken).ConfigureAwait(false);
    }
}
