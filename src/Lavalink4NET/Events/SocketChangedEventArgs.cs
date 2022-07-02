using System;

namespace Lavalink4NET.Events; 

/// <summary>
/// Event arguments for an event that indicates a player has changed socket.
/// </summary>
public class SocketChangedEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SocketChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldSocket">the old socket from player disconnected</param>
    /// <param name="newSocket">a new socket to which the player has attached</param>
    public SocketChangedEventArgs(LavalinkSocket oldSocket, LavalinkSocket newSocket)
    {
        OldSocket = oldSocket;
        NewSocket = newSocket;
    }

    /// <summary>
    /// The old socket from player disconnected
    /// </summary>
    public LavalinkSocket OldSocket { get; set; }
    
    
    /// <summary>
    /// A new socket to which the player has attached
    /// </summary>
    public LavalinkSocket NewSocket { get; set; }
}
