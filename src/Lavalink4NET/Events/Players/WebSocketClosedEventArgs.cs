namespace Lavalink4NET.Events.Players;

using System;
using System.Net.WebSockets;
using Lavalink4NET.Players;

public sealed class WebSocketClosedEventArgs : PlayerEventArgs
{
    public WebSocketClosedEventArgs(ILavalinkPlayer player, WebSocketCloseStatus closeCode, string reason, bool byRemote)
        : base(player)
    {
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(reason);

        CloseCode = closeCode;
        Reason = reason;
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
