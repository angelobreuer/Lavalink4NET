namespace Lavalink4NET.Players;

public delegate T PlayerFactory<out T>(PlayerProperties properties) where T : ILavalinkPlayer;