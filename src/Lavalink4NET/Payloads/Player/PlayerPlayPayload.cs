/*
 *  File:   PlayerPlayPayload.cs
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

namespace Lavalink4NET.Payloads.Player;

using System;
using System.Text.Json.Serialization;
using Lavalink4NET.Converters;

/// <summary>
///     The strongly-typed representation of a player play payload sent to the lavalink node (in
///     serialized JSON format). For more reference see https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md
/// </summary>
public sealed class PlayerPlayPayload
{
    /// <summary>
    ///     Gets the guild snowflake identifier the player update is for.
    /// </summary>
    [JsonPropertyName("guildId")]
    [JsonConverter(typeof(UInt64AsStringJsonSerializer))]
    public ulong GuildId { get; init; }

    /// <summary>
    ///     Gets the track identifier that the player should play.
    /// </summary>
    [JsonPropertyName("track")]
    public string TrackIdentifier { get; init; }

    /// <summary>
    ///     Gets the track start position in milliseconds.
    /// </summary>
    [JsonPropertyName("startTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan? StartTime { get; init; }

    /// <summary>
    ///     Gets the track end position in milliseconds.
    /// </summary>
    [JsonPropertyName("endTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan? EndTime { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the track play should be ignored if the same track
    ///     is currently playing.
    /// </summary>
    [JsonPropertyName("noReplace")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool NoReplace { get; init; }
}
