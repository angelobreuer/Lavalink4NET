namespace Lavalink4NET.Events;

using System;

/// <summary>
///     Event arguments for an event that indicates a connect action happened.
/// </summary>
public class ConnectionEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ConnectionEventArgs"/> class.
    /// </summary>
    /// <param name="uri">the URI connect / reconnected / disconnected from / to</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="uri"/> is <see langword="null"/>.
    /// </exception>
    public ConnectionEventArgs(Uri uri)
        => Uri = uri ?? throw new ArgumentNullException(nameof(uri));

    /// <summary>
    ///     Gets the URI connect / reconnected / disconnected from / to.
    /// </summary>
    public Uri Uri { get; }
}
