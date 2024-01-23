namespace Lavalink4NET.DSharpPlus;

using System.Text.Json.Serialization;

internal readonly record struct VoiceStateUpdatePayload
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; init; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; init; }

    [JsonPropertyName("self_mute")]
    public bool IsSelfMuted { get; init; }

    [JsonPropertyName("self_deaf")]
    public bool IsSelfDeafened { get; init; }

    public VoiceStateUpdatePayload(ulong guildId, ulong? channelId, bool isSelfMuted = false, bool isSelfDeafened = false)
    {
        GuildId = guildId;
        ChannelId = channelId;
        IsSelfMuted = isSelfMuted;
        IsSelfDeafened = isSelfDeafened;
    }
}
