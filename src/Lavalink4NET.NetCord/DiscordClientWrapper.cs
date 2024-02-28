namespace Lavalink4NET.NetCord;

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using global::NetCord.Gateway;
using Lavalink4NET.Clients;
using Lavalink4NET.Clients.Events;
using Lavalink4NET.Events;

public sealed class DiscordClientWrapper : IDiscordClientWrapper
{
    private readonly IDiscordClientWrapper _client;

    public DiscordClientWrapper(GatewayClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        _client = new SocketDiscordClientWrapper(client);
    }

    public DiscordClientWrapper(ShardedGatewayClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        _client = new ShardedDiscordClientWrapper(client);
    }

    public event AsyncEventHandler<VoiceServerUpdatedEventArgs>? VoiceServerUpdated
    {
        add => _client.VoiceServerUpdated += value;
        remove => _client.VoiceServerUpdated -= value;
    }

    public event AsyncEventHandler<VoiceStateUpdatedEventArgs>? VoiceStateUpdated
    {
        add => _client.VoiceStateUpdated += value;
        remove => _client.VoiceStateUpdated -= value;
    }

    public ValueTask<ImmutableArray<ulong>> GetChannelUsersAsync(ulong guildId, ulong voiceChannelId, bool includeBots = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return _client.GetChannelUsersAsync(guildId, voiceChannelId, includeBots, cancellationToken);
    }

    public ValueTask SendVoiceUpdateAsync(ulong guildId, ulong? voiceChannelId, bool selfDeaf = false, bool selfMute = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return _client.SendVoiceUpdateAsync(guildId, voiceChannelId, selfDeaf, selfMute, cancellationToken);
    }

    public ValueTask<ClientInformation> WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return _client.WaitForReadyAsync(cancellationToken);
    }
}
