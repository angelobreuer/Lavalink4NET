namespace Lavalink4NET.Payloads.Player;

using System.Text.Json.Serialization;
using Lavalink4NET.Converters;

/// <summary>
///     The strongly-typed representation of a player destroy payload sent to the lavalink node
///     (in serialized JSON format). For more reference see https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md
/// </summary>
public sealed class PlayerDestroyPayload
{
    /// <summary>
    ///     Gets the guild snowflake identifier where the player should be destroyed.
    /// </summary>
    [JsonPropertyName("guildId")]
    [JsonConverter(typeof(UInt64AsStringJsonSerializer))]
    public ulong GuildId { get; init; }
}
