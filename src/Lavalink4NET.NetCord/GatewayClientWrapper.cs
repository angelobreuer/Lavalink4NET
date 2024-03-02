namespace Lavalink4NET.NetCord;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using global::NetCord.Gateway;
using Lavalink4NET.Clients;

internal sealed class GatewayClientWrapper : GatewayClientWrapperBase, IDiscordClientWrapper, IDisposable
{
    private readonly GatewayClient _client;

    public GatewayClientWrapper(GatewayClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        _client = client;

        _client.VoiceStateUpdate += HandleVoiceStateUpdateAsync;
        _client.VoiceServerUpdate += HandleVoiceServerUpdateAsync;
    }

    public void Dispose()
    {
        _client.VoiceStateUpdate -= HandleVoiceStateUpdateAsync;
        _client.VoiceServerUpdate -= HandleVoiceServerUpdateAsync;
    }

    public override async ValueTask<ClientInformation> WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _client.ReadyAsync
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        var shardCount = _client.Shard?.Count ?? 1;

        return new ClientInformation("NetCord", _client.Id, shardCount);
    }

    protected override GatewayClient GetClient(ulong guildId) => _client;

    protected override bool TryGetGuild(ulong guildId, [MaybeNullWhen(false)] out Guild guild)
    {
        return _client.Cache.Guilds.TryGetValue(guildId, out guild);
    }
}
