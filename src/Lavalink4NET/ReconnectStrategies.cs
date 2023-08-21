namespace Lavalink4NET;

using System;

/// <summary>
///     A set of out-of-box reconnect strategies provided by Lavalink4NET.
/// </summary>
public static class ReconnectStrategies
{
    /// <summary>
    ///     The default reconnection strategy.
    /// </summary>
    public static ReconnectStrategy DefaultStrategy { get; } = (start, tries)
        => TimeSpan.FromSeconds(Math.Max(15, tries * 5));

    /// <summary>
    ///     A reconnection strategy that disables the reconnection.
    /// </summary>
    public static ReconnectStrategy None { get; } = (start, tries) => null;
}
