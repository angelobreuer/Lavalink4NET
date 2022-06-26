/*
 *  File:   EventType.cs
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

namespace Lavalink4NET.Payloads;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Lavalink4NET.Converters;

/// <summary>
///     The event types in lavalink event payloads.
/// </summary>
[JsonConverter(typeof(EventTypeJsonConverter))]
public readonly struct EventType : IEquatable<EventType>
{
    /// <summary>
    ///     Indicates that a playing track ended.
    /// </summary>
    public static readonly EventType TrackEnd = new("TrackEndEvent");

    /// <summary>
    ///     Indicates that an exception occurred while playing a track.
    /// </summary>
    public static readonly EventType TrackException = new("TrackExceptionEvent");

    /// <summary>
    ///     Indicates that a playing track started.
    /// </summary>
    public static readonly EventType TrackStart = new("TrackStartEvent");

    /// <summary>
    ///     Indicates that a track got stuck while playing.
    /// </summary>
    public static readonly EventType TrackStuck = new("TrackStuckEvent");

    /// <summary>
    ///     Indicates that the discord voice socket was closed.
    /// </summary>
    public static readonly EventType WebSocketClosed = new("WebSocketClosedEvent");

    public EventType(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Value { get; }
    public static bool operator !=(EventType left, EventType right)
    {
        return !(left == right);
    }

    public static bool operator ==(EventType left, EventType right)
    {
        return left.Equals(right);
    }

    public override bool Equals(object? obj)
    {
        return obj is EventType type && Equals(type);
    }

    public bool Equals(EventType other)
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return -1937169414 + EqualityComparer<string>.Default.GetHashCode(Value);
    }
}
