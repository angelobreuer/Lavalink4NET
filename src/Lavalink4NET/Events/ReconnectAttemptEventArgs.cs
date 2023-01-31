namespace Lavalink4NET.Events;

using System;

/// <summary>
/// </summary>
public sealed class ReconnectAttemptEventArgs : ConnectionEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ReconnectAttemptEventArgs"/> class.
    /// </summary>
    /// <param name="uri">the URI connect / reconnected / disconnected from / to</param>
    /// <param name="attempt">the number of reconnect attempts already made (1 = first)</param>
    /// <param name="strategy">the reconnect strategy used</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="uri"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="strategy"/> is <see langword="null"/>.
    /// </exception>
    public ReconnectAttemptEventArgs(Uri uri, int attempt, ReconnectStrategy strategy) : base(uri)
    {
        Attempt = attempt;
        Strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the reconnect attempt should be canceled.
    /// </summary>
    public bool CancelReconnect { get; set; }

    /// <summary>
    ///     Gets the number of reconnect attempts already made (1 = first).
    /// </summary>
    public int Attempt { get; }

    /// <summary>
    ///     Gets the reconnect strategy used.
    /// </summary>
    public ReconnectStrategy Strategy { get; }
}
