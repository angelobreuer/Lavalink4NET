namespace Lavalink4NET.NetCord;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using global::NetCord.Gateway;
using Lavalink4NET.Clients;
using Lavalink4NET.Clients.Events;
using Lavalink4NET.Events;

public sealed class DiscordClientWrapper : IDiscordClientWrapper, IDisposable
{
    private readonly GatewayClient _client;

    public DiscordClientWrapper(GatewayClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        _client = client;

        _client.VoiceStateUpdate += HandleVoiceStateUpdateAsync;
        _client.VoiceServerUpdate += HandleVoiceServerUpdateAsync;
    }

    public event AsyncEventHandler<VoiceServerUpdatedEventArgs>? VoiceServerUpdated;

    public event AsyncEventHandler<VoiceStateUpdatedEventArgs>? VoiceStateUpdated;

    public void Dispose()
    {
        _client.VoiceStateUpdate -= HandleVoiceStateUpdateAsync;
        _client.VoiceServerUpdate -= HandleVoiceServerUpdateAsync;
    }

    public ValueTask<ImmutableArray<ulong>> GetChannelUsersAsync(ulong guildId, ulong voiceChannelId, bool includeBots = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_client.Cache.Guilds.TryGetValue(guildId, out var guild))
        {
            return new ValueTask<ImmutableArray<ulong>>([]);
        }

        var voiceStates = guild.VoiceStates
            .Where(x => x.Value.ChannelId == voiceChannelId)
            .Where(x => x.Value.UserId != _client.Id);

        if (!includeBots)
        {
            voiceStates = voiceStates.Where(x => x.Value.User is not { IsBot: true, });
        }

        var userIds = voiceStates.Select(x => x.Value.UserId).ToImmutableArray();
        return new ValueTask<ImmutableArray<ulong>>(userIds);
    }

    public async ValueTask SendVoiceUpdateAsync(ulong guildId, ulong? voiceChannelId, bool selfDeaf = false, bool selfMute = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var voiceStateProperties = new VoiceStateProperties(guildId, voiceChannelId) { SelfDeaf = selfDeaf, SelfMute = selfMute, };

        await _client
            .UpdateVoiceStateAsync(voiceStateProperties)
            .ConfigureAwait(false);
    }

    public async ValueTask<ClientInformation> WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _client.ReadyAsync
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        var shardCount = _client.Shard?.Count ?? 1;

        return new ClientInformation("NetCord", _client.Id, shardCount);
    }

    private ValueTask HandleVoiceServerUpdateAsync(VoiceServerUpdateEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(eventArgs);

        if (eventArgs.Endpoint is null)
        {
            return default;
        }

        var voiceServerUpdatedEventArgs = new VoiceServerUpdatedEventArgs(
            guildId: eventArgs.GuildId,
            voiceServer: new VoiceServer(eventArgs.Token, eventArgs.Endpoint));

        return VoiceServerUpdated.InvokeAsync(this, voiceServerUpdatedEventArgs);
    }

    private async ValueTask HandleVoiceStateUpdateAsync(global::NetCord.Gateway.VoiceState eventArgs)
    {
        ArgumentNullException.ThrowIfNull(eventArgs);

        // Retrieve previous voice state from cache
        var previousVoiceState = _client.Cache.Guilds.TryGetValue(eventArgs.GuildId, out var guild)
            && guild.VoiceStates.TryGetValue(eventArgs.UserId, out var previousVoiceStateData)
            ? new Clients.VoiceState(VoiceChannelId: previousVoiceStateData.ChannelId, SessionId: previousVoiceStateData.SessionId)
            : default;

        var updatedVoiceState = new Clients.VoiceState(
            VoiceChannelId: eventArgs.ChannelId,
            SessionId: eventArgs.SessionId);

        var voiceStateUpdatedEventArgs = new VoiceStateUpdatedEventArgs(
            eventArgs.GuildId,
            eventArgs.UserId,
            eventArgs.UserId == _client.Id,
            updatedVoiceState,
            previousVoiceState);

        await VoiceStateUpdated
            .InvokeAsync(this, voiceStateUpdatedEventArgs)
            .ConfigureAwait(false);
    }
}
