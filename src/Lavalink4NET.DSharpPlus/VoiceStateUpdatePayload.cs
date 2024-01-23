namespace Lavalink4NET.DSharpPlus;

using Newtonsoft.Json;

internal sealed class VoiceStateUpdatePayload
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; init; }

    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; init; }

    [JsonProperty("self_mute")]
    public bool IsSelfMuted { get; init; }

    [JsonProperty("self_deaf")]
    public bool IsSelfDeafened { get; init; }
}
