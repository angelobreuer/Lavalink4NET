namespace Lavalink4NET.Events;

using System;

/// <summary>
///     The event arguments for the <see cref="LavalinkSocket.Connected"/> event.
/// </summary>
public class ConnectedEventArgs : ConnectionEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ConnectedEventArgs"/> class.
    /// </summary>
    /// <param name="uri">the URI connect / reconnected / disconnected from / to</param>
    /// <param name="wasReconnect">a value indicating whether the connect was a reconnect</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="uri"/> is <see langword="null"/>.
    /// </exception>
    public ConnectedEventArgs(Uri uri, bool wasReconnect) : base(uri)
        => WasReconnect = wasReconnect;

    /// <summary>
    ///     Gets a value indicating whether the connect was a reconnect.
    /// </summary>
    public bool WasReconnect { get; }
}
