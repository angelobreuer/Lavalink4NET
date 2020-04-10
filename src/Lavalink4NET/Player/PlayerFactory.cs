namespace Lavalink4NET.Player
{
    /// <summary>
    ///     A factory used to create players.
    /// </summary>
    /// <typeparam name="T">the type of the player</typeparam>
    /// <returns>the player</returns>
    public delegate T PlayerFactory<T>() where T : LavalinkPlayer;
}