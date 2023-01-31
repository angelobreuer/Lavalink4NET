namespace Lavalink4NET.Protocol.Payloads;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

public sealed record class PlayerUpdatePayload(
    [property: JsonRequired]
    [property: JsonPropertyName("guildId")]
    [property: JsonConverter(typeof(SnowflakeJsonConverter))]
    ulong GuildId,

    [property: JsonRequired]
    [property: JsonPropertyName("state")]
    PlayerStateModel State) : IPayload;