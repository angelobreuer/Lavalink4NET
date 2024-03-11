namespace Lavalink4NET.Events;

using System;
using System.Net.WebSockets;
using Lavalink4NET.Socket;

public sealed class ConnectionClosedEventArgs : EventArgs
{
    public ConnectionClosedEventArgs(
        ILavalinkSocket lavalinkSocket,
        WebSocketCloseStatus? closeStatus,
        string? closeStatusDescription,
        Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(lavalinkSocket);

        LavalinkSocket = lavalinkSocket;
        CloseStatus = closeStatus;
        CloseStatusDescription = closeStatusDescription;
        Exception = exception;
    }

    public ILavalinkSocket LavalinkSocket { get; }

    public WebSocketCloseStatus? CloseStatus { get; }

    public string? CloseStatusDescription { get; }

    public Exception? Exception { get; }

    public bool AllowReconnect { get; set; } = true;
}
