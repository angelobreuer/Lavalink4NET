namespace Lavalink4NET.Remora.Discord;

using System.Threading;
using System.Threading.Tasks;
using global::Remora.Discord.API.Abstractions.Gateway.Events;
using global::Remora.Discord.Gateway.Responders;
using global::Remora.Results;
using Lavalink4NET.Clients;

internal sealed class RemoraLavalinkEventResponder : IResponder<IReady>, IResponder<IVoiceServerUpdate>, IResponder<IGuildDelete>, IResponder<IChannelDelete>, IResponder<IVoiceStateUpdate>, IResponder<IGuildCreate>
{
    private readonly IDiscordVoiceEventProcessor _voiceEventProcessor;

    public RemoraLavalinkEventResponder(IDiscordVoiceEventProcessor voiceEventProcessor)
    {
        ArgumentNullException.ThrowIfNull(voiceEventProcessor);

        _voiceEventProcessor = voiceEventProcessor;
    }

    public async Task<Result> RespondAsync(IReady gatewayEvent, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(gatewayEvent);

        var shardCount = gatewayEvent.Shard.HasValue
            ? gatewayEvent.Shard.Value.ShardCount
            : 1;

        var clientInformation = new ClientInformation(
            Label: "Remora.Discord",
            CurrentUserId: gatewayEvent.User.ID.Value,
            ShardCount: shardCount);

        await _voiceEventProcessor
            .NotifyReadyAsync(clientInformation, ct)
            .ConfigureAwait(false);

        return Result.FromSuccess();
    }

    public async Task<Result> RespondAsync(IVoiceStateUpdate gatewayEvent, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(gatewayEvent);

        if (!gatewayEvent.GuildID.HasValue)
        {
            return Result.FromSuccess();
        }

        var voiceState = new VoiceState(
            VoiceChannelId: gatewayEvent.ChannelID?.Value,
            SessionId: gatewayEvent.SessionID);

        await _voiceEventProcessor
            .NotifyVoiceStateUpdatedAsync(gatewayEvent.GuildID.Value.Value, gatewayEvent.UserID.Value, voiceState, ct)
            .ConfigureAwait(false);

        return Result.FromSuccess();
    }

    public async Task<Result> RespondAsync(IChannelDelete gatewayEvent, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(gatewayEvent);

        if (!gatewayEvent.GuildID.HasValue)
        {
            return Result.FromSuccess();
        }

        await _voiceEventProcessor
            .NotifyChannelDeletedAsync(gatewayEvent.GuildID.Value.Value, gatewayEvent.ID.Value, ct)
            .ConfigureAwait(false);

        return Result.FromSuccess();
    }

    public async Task<Result> RespondAsync(IGuildDelete gatewayEvent, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(gatewayEvent);

        await _voiceEventProcessor
            .NotifyGuildDeletedAsync(gatewayEvent.ID.Value, ct)
            .ConfigureAwait(false);

        return Result.FromSuccess();
    }

    public async Task<Result> RespondAsync(IVoiceServerUpdate gatewayEvent, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(gatewayEvent);

        if (gatewayEvent.Endpoint is null)
        {
            return Result.FromSuccess();
        }

        var voiceServer = new VoiceServer(
            Endpoint: gatewayEvent.Endpoint,
            Token: gatewayEvent.Token);

        await _voiceEventProcessor
            .NotifyVoiceServerUpdatedAsync(gatewayEvent.GuildID.Value, voiceServer, ct)
            .ConfigureAwait(false);

        return Result.FromSuccess();
    }

    public async Task<Result> RespondAsync(IGuildCreate gatewayEvent, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(gatewayEvent);

        var guild = gatewayEvent.Guild.AsT0;

        if (guild is not null)
        {
            await _voiceEventProcessor
                .NotifyGuildCreatedAsync(guild, ct)
                .ConfigureAwait(false);
        }

        return Result.FromSuccess();
    }
}
