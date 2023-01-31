namespace Lavalink4NET.Payloads.Events;

using System;
using System.Text.Json.Serialization;

/// <summary>
///     The update data for the voice server update that is sent to the lavalink server when it
///     was received from the discord gateway.
/// </summary>
public sealed class VoiceServerUpdateEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="VoiceServerUpdateEvent"/> class.
    /// </summary>
    /// <param name="token">the token for the voice connection</param>
    /// <param name="guildId">the id of the guild the update is for</param>
    /// <param name="endpoint">the endpoint of the voice server</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="token"/> is <see langword="null"/>
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="endpoint"/> is <see langword="null"/>
    /// </exception>
    public VoiceServerUpdateEvent(string token, ulong guildId, string endpoint)
    {
        Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        Token = token ?? throw new ArgumentNullException(nameof(token));
        GuildId = guildId;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="VoiceServerUpdateEvent"/> class.
    /// </summary>
    /// <param name="voiceServer">the voice server</param>
    public VoiceServerUpdateEvent(VoiceServer voiceServer)
        : this(voiceServer.Token, voiceServer.GuildId, voiceServer.Endpoint)
    {
    }

    /// <summary>
    ///     Gets the token for the voice connection.
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the id of the guild the update is for
    /// </summary>
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; init; }

    /// <summary>
    ///     Gets the endpoint of the voice server.
    /// </summary>
    [JsonPropertyName("endpoint")]
    public string Endpoint { get; init; } = string.Empty;
}
