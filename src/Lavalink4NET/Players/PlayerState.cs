namespace Lavalink4NET.Players;

public enum PlayerState:byte
{
    Destroyed, // Destroyed/disposed
    NotPlaying, // Active but nothing playing
    Playing, // Active and playing
    Paused, // Paused
}
