/*
 *  File:   TrackLoadType.cs
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

namespace Lavalink4NET.Rest;

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
///     The possible track load types.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum TrackLoadType
{
    /// <summary>
    ///     A single track was loaded.
    /// </summary>
    [EnumMember(Value = "TRACK_LOADED")]
    TrackLoaded,

    /// <summary>
    ///     A playlist was loaded.
    /// </summary>
    [EnumMember(Value = "PLAYLIST_LOADED")]
    PlaylistLoaded,

    /// <summary>
    ///     A search result was made.
    /// </summary>
    [EnumMember(Value = "SEARCH_RESULT")]
    SearchResult,

    /// <summary>
    ///     No matches were found for the given identifier.
    /// </summary>
    [EnumMember(Value = "NO_MATCHES")]
    NoMatches,

    /// <summary>
    ///     Something happened while loading the track(s).
    /// </summary>
    [EnumMember(Value = "LOAD_FAILED")]
    LoadFailed
}
