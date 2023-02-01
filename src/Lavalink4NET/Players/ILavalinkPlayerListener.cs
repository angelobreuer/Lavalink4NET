namespace Lavalink4NET.Players;

public interface ILavalinkPlayerListener
{
    void NotifyChannelUpdate(ulong voiceChannelId);
}
