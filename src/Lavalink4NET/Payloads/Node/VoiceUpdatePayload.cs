namespace Lavalink4NET.Payloads.Node;

using System.Text.Json.Serialization;
using Lavalink4NET.Converters;
using Lavalink4NET.Payloads.Events;

/// <summary>
///     The representation of a voice update lavalink payload.
/// </summary>
public sealed class VoiceUpdatePayload
{
    /// <summary>
    ///     Gets the guild snowflake identifier the voice update is for.
    /// </summary>
    [JsonPropertyName("guildId")]
    [JsonConverter(typeof(UInt64AsStringJsonSerializer))]
    public ulong GuildId { get; init; }

    /// <summary>
    ///     Gets the discord voice state session identifier received from the voice state update payload.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string SessionId { get; init; } = null!;

    /// <summary>
    ///     Gets the voice server update event.
    /// </summary>
    [JsonPropertyName("event")]
    public VoiceServerUpdateEvent VoiceServerUpdateEvent { get; init; } = null!;
}
