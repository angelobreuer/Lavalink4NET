namespace Lavalink4NET.Player
{
    public delegate T PlayerFactory<T>() where T : LavalinkPlayer;
}