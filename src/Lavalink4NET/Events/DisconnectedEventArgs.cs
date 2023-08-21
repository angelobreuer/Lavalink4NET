namespace Lavalink4NET.Events;

using System;
using System.Net.WebSockets;

/// <summary>
///     The event arguments for the <see cref="LavalinkSocket.Disconnected"/> event.
/// </summary>
public class DisconnectedEventArgs : ConnectionEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DisconnectedEventArgs"/> class.
    /// </summary>
    /// <param name="uri">the URI connect / reconnected / disconnected from / to</param>
    /// <param name="closeStatus">the close status</param>
    /// <param name="reason">the close reason</param>
    /// <param name="byRemote">
    ///     a value indicating whether the connection was closed by the remote endpoint.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="uri"/> is <see langword="null"/>.
    /// </exception>
    public DisconnectedEventArgs(Uri uri, WebSocketCloseStatus closeStatus, string? reason, bool byRemote) : base(uri)
    {
        CloseStatus = closeStatus;
        Reason = reason;
        ByRemote = byRemote;
    }

    /// <summary>
    ///     Gets the close status.
    /// </summary>
    public WebSocketCloseStatus CloseStatus { get; }

    /// <summary>
    ///     Gets the close reason.
    /// </summary>
    public string? Reason { get; }

    /// <summary>
    ///     Gets a value indicating whether the connection was closed by the remote endpoint.
    /// </summary>
    public bool ByRemote { get; }
}
