namespace Lavalink4NET;

using System;

/// <summary>
///     The delegate for a reconnection strategy.
/// </summary>
/// <param name="start">the time when the reconnection started / the connection was lost</param>
/// <param name="tries">the number of tries to reconnect (one-based)</param>
/// <returns>
///     the time to wait to the next reconnect; or <see langword="null"/> if it is wanted to give
///     up reconnecting to the node.
/// </returns>
public delegate TimeSpan? ReconnectStrategy(DateTimeOffset start, int tries);
