namespace Lavalink4NET.Remora.Discord;

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using global::Remora.Discord.API.Gateway.Commands;
using global::Remora.Discord.Gateway;
using global::Remora.Rest.Core;
using Lavalink4NET.Clients;
using Lavalink4NET.Clients.Events;
using Lavalink4NET.Events;

internal sealed class DiscordClientWrapper : IDiscordClientWrapper, IDiscordVoiceEventProcessor
{
    private readonly TaskCompletionSource<ClientInformation> _clientInformationTaskCompletionSource;
    private readonly DiscordGatewayClient _client;

    // Key = GuildId, Value = { Key = UserId, Value = VoiceChannelId }
    private readonly ConcurrentDictionary<ulong, IImmutableDictionary<ulong, ulong>> _voiceStateCache;

    public DiscordClientWrapper(DiscordGatewayClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        _clientInformationTaskCompletionSource = new TaskCompletionSource<ClientInformation>(
            creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);

        _client = client;

        _voiceStateCache = new ConcurrentDictionary<ulong, IImmutableDictionary<ulong, ulong>>();
    }

    public event AsyncEventHandler<VoiceServerUpdatedEventArgs>? VoiceServerUpdated;

    public event AsyncEventHandler<VoiceStateUpdatedEventArgs>? VoiceStateUpdated;

    public ValueTask<ImmutableArray<ulong>> GetChannelUsersAsync(ulong guildId, ulong voiceChannelId, bool includeBots = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_voiceStateCache.TryGetValue(guildId, out var voiceStateCache))
        {
            return ValueTask.FromResult(ImmutableArray<ulong>.Empty);
        }

        var userIds = voiceStateCache
            .Where(x => x.Value == voiceChannelId)
            .Select(x => x.Key)
            .ToImmutableArray();

        return ValueTask.FromResult(userIds);
    }

    public ValueTask SendVoiceUpdateAsync(ulong guildId, ulong? voiceChannelId, bool selfDeaf = false, bool selfMute = false, CancellationToken cancellationToken = default)
    {
        var channelId = voiceChannelId is null
            ? default(Snowflake?)
            : new Snowflake(voiceChannelId.Value);

        var voiceStateUpdateCommand = new UpdateVoiceState(
            GuildID: new Snowflake(guildId),
            IsSelfMuted: selfMute,
            IsSelfDeafened: selfDeaf,
            ChannelID: channelId);

        _client.SubmitCommand(voiceStateUpdateCommand);

        return default;
    }

    public ValueTask<ClientInformation> WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var task = _clientInformationTaskCompletionSource.Task.WaitAsync(cancellationToken);
        return new ValueTask<ClientInformation>(task);
    }

    ValueTask IDiscordVoiceEventProcessor.NotifyReadyAsync(ClientInformation clientInformation, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _clientInformationTaskCompletionSource.TrySetResult(clientInformation);
        return default;
    }

    async ValueTask IDiscordVoiceEventProcessor.NotifyVoiceServerUpdatedAsync(ulong guildId, VoiceServer voiceServer, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var eventArgs = new VoiceServerUpdatedEventArgs(guildId, voiceServer);

        await VoiceServerUpdated
            .InvokeAsync(this, eventArgs)
            .ConfigureAwait(false);
    }

    ValueTask IDiscordVoiceEventProcessor.NotifyGuildDeletedAsync(ulong guildId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _voiceStateCache.Remove(guildId, out _);
        return default;
    }

    ValueTask IDiscordVoiceEventProcessor.NotifyChannelDeletedAsync(ulong guildId, ulong channelId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        static IImmutableDictionary<ulong, ulong> AddValueFactory(ulong guildId)
            => ImmutableDictionary<ulong, ulong>.Empty;

        IImmutableDictionary<ulong, ulong> UpdateValueFactory(ulong guildId, IImmutableDictionary<ulong, ulong> value) => value
            .RemoveRange(value.Where(x => x.Value == channelId)
            .Select(x => x.Key));

        _voiceStateCache.AddOrUpdate(guildId, AddValueFactory, UpdateValueFactory);

        return default;
    }

    async ValueTask IDiscordVoiceEventProcessor.NotifyVoiceStateUpdatedAsync(ulong guildId, ulong userId, VoiceState voiceState, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var channelId = voiceState.VoiceChannelId;

        if (channelId is null)
        {
            static IImmutableDictionary<ulong, ulong> AddValueFactory(ulong guildId)
                => ImmutableDictionary<ulong, ulong>.Empty;

            IImmutableDictionary<ulong, ulong> UpdateValueFactory(ulong guildId, IImmutableDictionary<ulong, ulong> value)
                => value.Remove(userId);

            _voiceStateCache.AddOrUpdate(guildId, AddValueFactory, UpdateValueFactory);
        }
        else
        {
            IImmutableDictionary<ulong, ulong> AddValueFactory(ulong guildId)
                => ImmutableDictionary<ulong, ulong>.Empty.Add(userId, channelId.Value);

            IImmutableDictionary<ulong, ulong> UpdateValueFactory(ulong guildId, IImmutableDictionary<ulong, ulong> value)
                => value.Add(userId, channelId.Value);

            _voiceStateCache.AddOrUpdate(guildId, AddValueFactory, UpdateValueFactory);
        }

        // Task is already completed when this method is called
        Debug.Assert(_clientInformationTaskCompletionSource.Task.IsCompleted);

        if (userId == _clientInformationTaskCompletionSource.Task.Result.CurrentUserId)
        {
            var eventArgs = new VoiceStateUpdatedEventArgs(guildId, voiceState);

            await VoiceStateUpdated
                .InvokeAsync(this, eventArgs)
                .ConfigureAwait(false);
        }
    }

    internal bool TryGetUserChannelId(ulong guildId, ulong userId, out ulong voiceChannelId)
    {
        if (!_voiceStateCache.TryGetValue(guildId, out var voiceStateCache) ||
            !voiceStateCache.TryGetValue(userId, out voiceChannelId))
        {
            voiceChannelId = default;
            return false;
        }

        return true;
    }

    public ValueTask NotifyGuildCreatedAsync(global::Remora.Discord.API.Abstractions.Gateway.Events.IGuildCreate.IAvailableGuild guild, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(guild);

        var voiceStateMap = guild.VoiceStates
            .Where(x => x.ChannelID.HasValue && x.UserID.HasValue)
            .ToImmutableDictionary(x => x.UserID.Value.Value, x => x.ChannelID.Value!.Value.Value);

        _voiceStateCache[guild.ID.Value] = voiceStateMap;

        return default;
    }
}
