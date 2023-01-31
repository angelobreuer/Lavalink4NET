namespace Lavalink4NET.Events;

/// <summary>
///     Represents the event arguments for the <see
///     cref="IDiscordClientWrapper.VoiceStateUpdated"/> event.
/// </summary>
public sealed class VoiceStateUpdateEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="VoiceStateUpdateEventArgs"/> class.
    /// </summary>
    /// <param name="userId">the user snowflake identifier the update is for</param>
    /// <param name="voiceState">the new user voice state</param>
    public VoiceStateUpdateEventArgs(ulong userId, VoiceState voiceState)
    {
        UserId = userId;
        VoiceState = voiceState;
    }

    /// <summary>
    ///     Gets the user snowflake identifier the update is for.
    /// </summary>
    public ulong UserId { get; }

    /// <summary>
    ///     Gets the new user voice state.
    /// </summary>
    public VoiceState VoiceState { get; }
}
