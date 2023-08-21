namespace Lavalink4NET.DSharpPlus;

using System.Text.Json.Serialization;

internal sealed class VoiceStateUpdatePayload
{
    public VoiceStateUpdatePayload(ulong guildId, ulong? channelId, bool isSelfMuted = false, bool isSelfDeafened = false)
    {
        GuildId = guildId;
        ChannelId = channelId;
        IsSelfMuted = isSelfMuted;
        IsSelfDeafened = isSelfDeafened;
    }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; }

    [JsonPropertyName("self_mute")]
    public bool IsSelfMuted { get; }

    [JsonPropertyName("self_deaf")]
    public bool IsSelfDeafened { get; }
}
