/*
 *  File:   TrackLoadResponsePayload.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2020
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

namespace Lavalink4NET.Rest
{
    using Newtonsoft.Json;
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
        [JsonProperty("exception")]
        public TrackException Exception { get; internal set; } = null!;

        /// <summary>
        ///     Gets the type of what was loaded.
        /// </summary>
        [JsonRequired, JsonProperty("loadType")]
        public TrackLoadType LoadType { get; internal set; }

        /// <summary>
        ///     Gets the information about the playlist.
        /// </summary>
        [JsonRequired, JsonProperty("playlistInfo")]
        public PlaylistInfo? PlaylistInfo { get; internal set; }

        /// <summary>
        ///     Gets the loaded tracks.
        /// </summary>
        [JsonRequired, JsonProperty("tracks")]
        public LavalinkTrack[]? Tracks { get; internal set; }
    }
}
