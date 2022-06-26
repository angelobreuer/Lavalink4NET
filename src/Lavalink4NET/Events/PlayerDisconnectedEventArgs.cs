/*
 *  File:   PlayerDisconnectedEventArgs.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2022
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
using Lavalink4NET.Player;

/// <summary>
///     Event arguments for the <see cref="LavalinkNode.PlayerDisconnected"/> event.
/// </summary>
public sealed class PlayerDisconnectedEventArgs : PlayerEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PlayerDisconnectedEventArgs"/> class.
    /// </summary>
    /// <param name="player">the affected player</param>
    /// <param name="voiceChannelId">
    ///     the snowflake identifier of the voice channel disconnected from
    /// </param>
    /// <param name="disconnectCause">the reason why the player disconnected</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="player"/> is <see langword="null"/>.
    /// </exception>
    public PlayerDisconnectedEventArgs(LavalinkPlayer player, ulong voiceChannelId, PlayerDisconnectCause disconnectCause) : base(player)
    {
        VoiceChannelId = voiceChannelId;
        DisconnectCause = disconnectCause;
    }

    /// <summary>
    ///     Gets the reason why the player disconnected.
    /// </summary>
    public PlayerDisconnectCause DisconnectCause { get; }

    /// <summary>
    ///     Gets the snowflake identifier of the voice channel disconnected from.
    /// </summary>
    public ulong VoiceChannelId { get; }
}
