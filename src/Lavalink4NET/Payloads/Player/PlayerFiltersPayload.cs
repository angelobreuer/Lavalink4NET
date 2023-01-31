namespace Lavalink4NET.Payloads.Player;

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Lavalink4NET.Converters;

/// <summary>
///     The strongly-typed representation of a player filters update payload sent to the
///     lavalink node (in serialized JSON format). For more reference see the lavalink client
///     implementation documentation https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md
/// </summary>
public sealed class PlayerFiltersPayload
{
    /// <summary>
    ///     Gets the guild snowflake identifier the player equalizer update is for.
    /// </summary>
    [JsonPropertyName("guildId")]
    [JsonConverter(typeof(UInt64AsStringJsonSerializer))]
    public ulong GuildId { get; init; }

    [JsonExtensionData, JsonPropertyName("filters")]
    public Dictionary<string, object?> Filters { get; set; }
}
