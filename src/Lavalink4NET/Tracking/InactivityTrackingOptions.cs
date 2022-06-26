/*
 *  File:   InactivityTrackingOptions.cs
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

namespace Lavalink4NET.Tracking;

using System;

/// <summary>
///     The options for the <see cref="InactivityTrackingService"/>.
/// </summary>
public sealed class InactivityTrackingOptions
{
    /// <summary>
    ///     Gets or sets a value indicating whether the first track (after using
    ///     <see cref="InactivityTrackingService.BeginTracking"/>) should be delayed using the <see cref="PollInterval"/>.
    /// </summary>
    /// <remarks>This property defaults to <see langword="true"/>.</remarks>
    public bool DelayFirstTrack { get; set; } = true;

    /// <summary>
    ///     Gets or sets the delay for a player stop. Use <see cref="TimeSpan.Zero"/> for
    ///     disconnect immediately from the channel.
    /// </summary>
    /// <remarks>This property defaults to <c>TimeSpan.FromSeconds(30)</c></remarks>
    public TimeSpan DisconnectDelay { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    ///     Gets or sets the poll interval for the <see cref="InactivityTrackingService"/> in
    ///     which the players should be tested for inactivity. This also affects the <see cref="DisconnectDelay"/>.
    /// </summary>
    /// <remarks>This property defaults to <c>TimeSpan.FromSeconds(5)</c></remarks>
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    ///     Gets or sets a value indicating whether the <see cref="InactivityTrackingService"/>
    ///     should start tracking inactive players after constructing it.
    /// </summary>
    /// <remarks>This property defaults to <see langword="false"/>.</remarks>
    public bool TrackInactivity { get; set; }
}
