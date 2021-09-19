/*
 *  File:   DisconnectedEventArgs.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2021
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */

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
