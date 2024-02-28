namespace Lavalink4NET.NetCord;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using global::NetCord.Gateway;
using Lavalink4NET.Clients;

internal sealed class ShardedDiscordClientWrapper : DiscordClientWrapperBase, IDiscordClientWrapper, IDisposable
{
    private readonly ShardedGatewayClient _client;
    private readonly TaskCompletionSource _readyTaskCompletionSource;

    public ShardedDiscordClientWrapper(ShardedGatewayClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        _client = client;

        _readyTaskCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        _client.VoiceStateUpdate += HandleVoiceStateUpdateAsync;
        _client.VoiceServerUpdate += HandleVoiceServerUpdateAsync;
        _client.Ready += HandleShardReadyAsync;
    }

    private ValueTask HandleShardReadyAsync(GatewayClient client, ReadyEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(eventArgs);

        _readyTaskCompletionSource.TrySetResult();

        return default;
    }

    private ValueTask HandleVoiceServerUpdateAsync(GatewayClient client, VoiceServerUpdateEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(args);

        return HandleVoiceServerUpdateAsync(args);
    }

    private ValueTask HandleVoiceStateUpdateAsync(GatewayClient client, global::NetCord.Gateway.VoiceState state)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(state);

        return HandleVoiceStateUpdateAsync(state);
    }

    public void Dispose()
    {
        _client.VoiceStateUpdate -= HandleVoiceStateUpdateAsync;
        _client.VoiceServerUpdate -= HandleVoiceServerUpdateAsync;
    }

    public override async ValueTask<ClientInformation> WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _readyTaskCompletionSource.Task
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        return new ClientInformation("NetCord", _client.Id, _client.Count);
    }

    protected override GatewayClient GetClient(ulong guildId) => _client[guildId: guildId];

    protected override bool TryGetGuild(ulong guildId, [MaybeNullWhen(false)] out Guild guild)
    {
        return GetClient(guildId).Cache.Guilds.TryGetValue(guildId, out guild);
    }
}
