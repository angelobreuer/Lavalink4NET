/*
 *  File:   InactivePlayerEventArgs.cs
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

namespace Lavalink4NET.Tracking;

using System;
using Lavalink4NET.Player;

/// <summary>
///     The event arguments for the <see cref="InactivityTrackingService.InactivePlayer"/>.
/// </summary>
public sealed class InactivePlayerEventArgs : EventArgs
{
    /// <summary>
    ///     Initialize a new instance of the <see cref="InactivePlayerEventArgs"/> class.
    /// </summary>
    /// <param name="audioService">the audio service</param>
    /// <param name="player">the affected player</param>
    public InactivePlayerEventArgs(IAudioService audioService, LavalinkPlayer player)
    {
        AudioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
        Player = player ?? throw new ArgumentNullException(nameof(player));
    }

    /// <summary>
    ///     Gets the audio service.
    /// </summary>
    public IAudioService AudioService { get; }

    /// <summary>
    ///     Gets the affected player.
    /// </summary>
    public LavalinkPlayer Player { get; }

    /// <summary>
    ///     Gets a value indicating whether the player should be stopped.
    /// </summary>
    public bool ShouldStop { get; set; } = true;
}
