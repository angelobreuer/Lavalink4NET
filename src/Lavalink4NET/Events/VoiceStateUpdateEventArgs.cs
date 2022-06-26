/*
 *  File:   VoiceStateUpdateEventArgs.cs
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

/// <summary>
///     Represents the event arguments for the <see
///     cref="IDiscordClientWrapper.VoiceStateUpdated"/> event.
/// </summary>
public sealed class VoiceStateUpdateEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="VoiceStateUpdateEventArgs"/> class.
    /// </summary>
    /// <param name="userId">the user snowflake identifier the update is for</param>
    /// <param name="voiceState">the new user voice state</param>
    public VoiceStateUpdateEventArgs(ulong userId, VoiceState voiceState)
    {
        UserId = userId;
        VoiceState = voiceState;
    }

    /// <summary>
    ///     Gets the user snowflake identifier the update is for.
    /// </summary>
    public ulong UserId { get; }

    /// <summary>
    ///     Gets the new user voice state.
    /// </summary>
    public VoiceState VoiceState { get; }
}
