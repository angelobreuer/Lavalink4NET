namespace Lavalink4NET;

/// <summary>
///     Represents the information for a discord user voice state.
/// </summary>
public sealed class VoiceState
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="VoiceState"/> class.
    /// </summary>
    /// <param name="voiceChannelId">the voice channel identifier</param>
    /// <param name="guildId">
    ///     the guild snowflake identifier the voice state update is for
    /// </param>
    /// <param name="voiceSessionId">the voice session identifier</param>
    public VoiceState(ulong? voiceChannelId, ulong guildId, string voiceSessionId)
    {
        GuildId = guildId;
        VoiceChannelId = voiceChannelId;
        VoiceSessionId = voiceSessionId;
    }

    /// <summary>
    ///     Gets the voice channel identifier.
    /// </summary>
    public ulong? VoiceChannelId { get; }

    /// <summary>
    ///     Gets the guild snowflake identifier the voice state update is for.
    /// </summary>
    public ulong GuildId { get; }

    /// <summary>
    ///     Gets the voice session identifier.
    /// </summary>
    public string VoiceSessionId { get; }
}
