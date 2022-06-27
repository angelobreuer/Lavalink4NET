/*
 *  File:   LavalinkTrackInfo.cs
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

namespace Lavalink4NET.Player;

using System;
using System.Text.Json.Serialization;
using Lavalink4NET.Converters;
using Lavalink4NET.Decoding;

/// <summary>
///     The information store for a lavalink track.
/// </summary>
public sealed class LavalinkTrackInfo
{
    /// <summary>
    ///     Gets the name of the track author.
    /// </summary>
    [JsonPropertyName("author")]
    public string Author { get; init; }

    /// <summary>
    ///     Gets the duration of the track.
    /// </summary>
    [JsonPropertyName("length"), JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan Duration { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the track is a live stream.
    /// </summary>
    [JsonPropertyName("isStream")]
    public bool IsLiveStream { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the track is seek-able.
    /// </summary>
    [JsonPropertyName("isSeekable")]
    public bool IsSeekable { get; init; }

    /// <summary>
    ///     Gets the start position of the track.
    /// </summary>
    [JsonPropertyName("position"), JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan Position { get; init; }

    /// <summary>
    ///     Gets the track source.
    /// </summary>
    [JsonPropertyName("uri")]
    public string? Source { get; init; }

    /// <summary>
    ///     Gets the title of the track.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; init; }

    /// <summary>
    ///     Gets the unique track identifier (Example: dQw4w9WgXcQ, YouTube Video ID).
    /// </summary>
    [JsonPropertyName("identifier")]
    public string TrackIdentifier { get; init; }

    public LavalinkTrack CreateTrack()
    {
        var identifier = TrackEncoder.Encode(this);
        return new LavalinkTrack(identifier, this);
    }
}
