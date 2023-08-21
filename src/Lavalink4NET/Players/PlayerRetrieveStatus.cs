namespace Lavalink4NET.Players;

public enum PlayerRetrieveStatus : byte
{
    Success,
    UserNotInVoiceChannel,
    VoiceChannelMismatch,
    BotNotConnected,
    PreconditionFailed,
}
