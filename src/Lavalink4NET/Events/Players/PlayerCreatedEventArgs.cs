namespace Lavalink4NET.Events.Players;

using Lavalink4NET.Players;

public sealed class PlayerCreatedEventArgs : PlayerEventArgs
{
	public PlayerCreatedEventArgs(ILavalinkPlayer player)
		: base(player)
	{
	}
}
