namespace Lavalink4NET.Clients;

public enum MemberVoiceStateBehavior : byte
{
    /// <summary>
    ///     If the player is not connected, UserNotInVoiceChannel is returned.
    /// </summary>
    Ignore,

    /// <summary>
    ///     Regardless of the player's connection state, the user must be in a voice channel.
    /// </summary>
    AlwaysRequired,

    /// <summary>
    ///     Regardless of the player's connection state, the user must be in the same voice channel as the player.
    /// </summary>
    RequireSame,
}
