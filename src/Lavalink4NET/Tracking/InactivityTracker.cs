namespace Lavalink4NET.Tracking
{
    using System.Threading.Tasks;
    using Lavalink4NET.Player;

    /// <summary>
    ///     A delegate for an asynchronous player inactivity tracker.
    /// </summary>
    /// <param name="player">the player to check for inactivity</param>
    /// <param name="client">the discord client wrapper</param>
    /// <returns>
    ///     a task that represents the asynchronous task. The task result is a value indicating
    ///     whether the specified <paramref name="player"/> is inactive.
    /// </returns>
    public delegate Task<bool> InactivityTracker(LavalinkPlayer player, IDiscordClientWrapper client);
}