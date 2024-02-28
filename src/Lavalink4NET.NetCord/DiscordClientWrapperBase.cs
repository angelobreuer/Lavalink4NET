namespace Lavalink4NET.NetCord;

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using global::NetCord.Gateway;
using Lavalink4NET.Clients;
using Lavalink4NET.Clients.Events;
using Lavalink4NET.Events;

internal abstract class DiscordClientWrapperBase : IDiscordClientWrapper
{
    public event AsyncEventHandler<VoiceServerUpdatedEventArgs>? VoiceServerUpdated;

    public event AsyncEventHandler<VoiceStateUpdatedEventArgs>? VoiceStateUpdated;

    public ValueTask<ImmutableArray<ulong>> GetChannelUsersAsync(ulong guildId, ulong voiceChannelId, bool includeBots = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!TryGetGuild(guildId, out var guild))
        {
            return new ValueTask<ImmutableArray<ulong>>([]);
        }

        var currentUserId = GetClient(guildId).Id;

        var voiceStates = guild.VoiceStates
            .Where(x => x.Value.ChannelId == voiceChannelId)
            .Where(x => x.Value.UserId != currentUserId);

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

        await GetClient(guildId)
            .UpdateVoiceStateAsync(voiceStateProperties)
            .ConfigureAwait(false);
    }

    protected abstract bool TryGetGuild(ulong guildId, [MaybeNullWhen(false)] out Guild guild);

    protected abstract GatewayClient GetClient(ulong guildId);

    protected ValueTask HandleVoiceServerUpdateAsync(VoiceServerUpdateEventArgs eventArgs)
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

    protected async ValueTask HandleVoiceStateUpdateAsync(global::NetCord.Gateway.VoiceState eventArgs)
    {
        ArgumentNullException.ThrowIfNull(eventArgs);

        // Retrieve previous voice state from cache
        var previousVoiceState = TryGetGuild(eventArgs.GuildId, out var guild)
            && guild.VoiceStates.TryGetValue(eventArgs.UserId, out var previousVoiceStateData)
            ? new Clients.VoiceState(VoiceChannelId: previousVoiceStateData.ChannelId, SessionId: previousVoiceStateData.SessionId)
            : default;

        var currentUserId = GetClient(eventArgs.GuildId).Id;

        var updatedVoiceState = new Clients.VoiceState(
            VoiceChannelId: eventArgs.ChannelId,
            SessionId: eventArgs.SessionId);

        var voiceStateUpdatedEventArgs = new VoiceStateUpdatedEventArgs(
            eventArgs.GuildId,
            eventArgs.UserId,
            eventArgs.UserId == currentUserId,
            updatedVoiceState,
            previousVoiceState);

        await VoiceStateUpdated
            .InvokeAsync(this, voiceStateUpdatedEventArgs)
            .ConfigureAwait(false);
    }

    public abstract ValueTask<ClientInformation> WaitForReadyAsync(CancellationToken cancellationToken = default);
}
