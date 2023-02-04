namespace Lavalink4NET.Players;

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Protocol.Requests;
using Lavalink4NET.Rest;

internal sealed class LavalinkPlayerHandle
{
    private VoiceServer? _voiceServer;
    private VoiceState? _voiceState;
    private readonly ulong _guildId;
    private readonly IDiscordClientWrapper _client;
    private readonly ILavalinkApiClient _apiClient;
    private readonly string _sessionId;
    private readonly Func<PlayerProperties, ILavalinkPlayer> _playerFactory;
    private object _value;

    public LavalinkPlayerHandle(
        ulong guildId,
        IDiscordClientWrapper client,
        ILavalinkApiClient apiClient, // TODO: aggregate session id, client and api client
        string sessionId,
        Func<PlayerProperties, ILavalinkPlayer> playerFactory)
    {
        ArgumentNullException.ThrowIfNull(apiClient);
        ArgumentNullException.ThrowIfNull(sessionId);
        ArgumentNullException.ThrowIfNull(playerFactory);

        _value = new TaskCompletionSource<ILavalinkPlayer>(TaskCreationOptions.RunContinuationsAsynchronously);
        _guildId = guildId;
        _client = client;
        _apiClient = apiClient;
        _sessionId = sessionId;
        _playerFactory = playerFactory;
    }

    public ILavalinkPlayer? Player => _value as ILavalinkPlayer;

    public async ValueTask UpdateVoiceServerAsync(VoiceServer voiceServer, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(voiceServer);

        _voiceServer = voiceServer;

        if (_voiceState is not null)
        {
            await CompleteAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async ValueTask UpdateVoiceServerAsync(VoiceState voiceState, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(voiceState);

        _voiceState = voiceState;

        if (_voiceServer is not null)
        {
            await CompleteAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private async ValueTask CompleteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Debug.Assert(_voiceServer is not null);
        Debug.Assert(_voiceState is not null);

        var playerProperties = new PlayerUpdateProperties
        {
            VoiceState = new VoiceStateProperties(
                Token: _voiceServer.Value.Token,
                Endpoint: _voiceServer.Value.Endpoint,
                SessionId: _voiceState.Value.SessionId),
        };

        var model = await _apiClient
            .UpdatePlayerAsync(_sessionId, _guildId, playerProperties, cancellationToken)
            .ConfigureAwait(false);

        if (_value is TaskCompletionSource<ILavalinkPlayer> taskCompletionSource)
        {
            var properties = new PlayerProperties(
                ApiClient: _apiClient,
                Client: _client,
                GuildId: _guildId,
                VoiceChannelId: _voiceState.Value.VoiceChannelId.Value,
                SessionId: _sessionId,
                Model: model,
                DisconnectOnStop: false); // TODO: disconnect on stop

            var player = _playerFactory(properties);
            taskCompletionSource.TrySetResult(player);

            _value = player;
        }

        if (_value is ILavalinkPlayerListener playerListener)
        {
            playerListener.NotifyChannelUpdate(_voiceState.Value.VoiceChannelId.Value);
        }
    }

    public ValueTask<ILavalinkPlayer> GetPlayerAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_value is TaskCompletionSource<ILavalinkPlayer> taskCompletionSource)
        {
            return new ValueTask<ILavalinkPlayer>(taskCompletionSource.Task);
        }

        return ValueTask.FromResult(Unsafe.As<object, ILavalinkPlayer>(ref Unsafe.AsRef(_value)));
    }
}
