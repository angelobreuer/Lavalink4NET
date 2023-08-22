namespace Lavalink4NET.Events.Players;

using Lavalink4NET.Players;

public sealed class PlayerDestroyedEventArgs : PlayerEventArgs
{
    public PlayerDestroyedEventArgs(ILavalinkPlayer player)
        : base(player)
    {
    }
}
