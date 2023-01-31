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
