namespace Lavalink4NET.Remora.Discord;

using global::Remora.Discord.API.Abstractions.Gateway.Events;
using Lavalink4NET.Clients;

internal interface IDiscordVoiceEventProcessor
{
    ValueTask NotifyReadyAsync(ClientInformation clientInformation, CancellationToken cancellationToken = default);

    ValueTask NotifyVoiceServerUpdatedAsync(ulong guildId, VoiceServer voiceServer, CancellationToken cancellationToken = default);

    ValueTask NotifyGuildDeletedAsync(ulong guildId, CancellationToken cancellationToken = default);

    ValueTask NotifyChannelDeletedAsync(ulong guildId, ulong channelId, CancellationToken cancellationToken = default);

    ValueTask NotifyVoiceStateUpdatedAsync(ulong guildId, ulong userId, VoiceState voiceState, CancellationToken cancellationToken = default);

    ValueTask NotifyGuildCreatedAsync(IGuildCreate.IAvailableGuild guild, CancellationToken cancellationToken = default);
}
