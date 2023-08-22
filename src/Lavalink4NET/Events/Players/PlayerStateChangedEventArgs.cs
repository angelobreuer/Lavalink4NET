namespace Lavalink4NET.Events.Players;

using Lavalink4NET.Players;

public sealed class PlayerStateChangedEventArgs : PlayerEventArgs
{
    public PlayerStateChangedEventArgs(ILavalinkPlayer player, PlayerState state)
        : base(player)
    {
        State = state;
    }

    public PlayerState State { get; }
}
