namespace Lavalink4NET.Tracking;

using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Players;

/// <summary>
///     A delegate for an asynchronous player inactivity tracker.
/// </summary>
/// <param name="player">the player to check for inactivity</param>
/// <param name="client">the discord client wrapper</param>
/// <returns>
///     a task that represents the asynchronous task. The task result is a value indicating
///     whether the specified <paramref name="player"/> is inactive.
/// </returns>
public delegate ValueTask<bool> InactivityTracker(ILavalinkPlayer player, IDiscordClientWrapper client, CancellationToken cancellationToken = default);
