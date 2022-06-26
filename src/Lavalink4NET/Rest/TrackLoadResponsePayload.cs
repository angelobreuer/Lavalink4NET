/*
 *  File:   TrackLoadResponsePayload.cs
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

namespace Lavalink4NET.Rest;

using System.Text.Json.Serialization;
using Player;

/// <summary>
///     t The RESTful api HTTP response object for request to the "/tracks" endpoint.
/// </summary>
public sealed class TrackLoadResponsePayload
{
    /// <summary>
    ///     Gets an exception that indicates why the track load failed (see: <see cref="LoadType"/>).
    /// </summary>
    /// <remarks>
    ///     This property is only available if <see cref="TrackLoadType"/> is <see cref="TrackLoadType.LoadFailed"/>.
    /// </remarks>
    [JsonPropertyName("exception")]
    public TrackLoadException? Exception { get; init; }

    /// <summary>
    ///     Gets the type of what was loaded.
    /// </summary>
    [JsonPropertyName("loadType")]
    public TrackLoadType LoadType { get; init; }

    /// <summary>
    ///     Gets the information about the playlist.
    /// </summary>
    [JsonPropertyName("playlistInfo")]
    public PlaylistInfo? PlaylistInfo { get; init; }

    /// <summary>
    ///     Gets the loaded tracks.
    /// </summary>
    [JsonPropertyName("tracks")]
    public LavalinkTrack[]? Tracks { get; init; }
}
