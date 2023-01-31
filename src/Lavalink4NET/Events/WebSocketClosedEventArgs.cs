namespace Lavalink4NET.Events;

using System;
using System.Net.WebSockets;

public sealed class WebSocketClosedEventArgs : EventArgs
{
    public WebSocketClosedEventArgs(WebSocketCloseStatus closeCode, string reason, bool byRemote)
    {
        CloseCode = closeCode;
        Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        ByRemote = byRemote;
    }

    /// <summary>
    ///     Gets a value indicating whether the connection was closed by the remote (discord gateway).
    /// </summary>
    public bool ByRemote { get; }

    /// <summary>
    ///     Gets the web-socket close code.
    /// </summary>
    public WebSocketCloseStatus CloseCode { get; }

    /// <summary>
    ///     Gets the reason why the web-socket was closed.
    /// </summary>
    public string Reason { get; }
}
