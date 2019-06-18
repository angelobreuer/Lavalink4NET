/*
 *  File:   StreamProvider.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2019
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

namespace Lavalink4NET.Player
{
    /// <summary>
    ///     A set of different stream providers supported by lavaplayer (https://github.com/sedmelluq/lavaplayer).
    /// </summary>
    public enum StreamProvider
    {
        /// <summary>
        ///     Unknown stream provider.
        /// </summary>
        Unknown,

        /// <summary>
        ///     A stream from YouTube.
        /// </summary>
        YouTube,

        /// <summary>
        ///     A stream from SoundCloud.
        /// </summary>
        SoundCloud,

        /// <summary>
        ///     A stream from Bandcamp.
        /// </summary>
        Bandcamp,

        /// <summary>
        ///     A stream from Vimeo.
        /// </summary>
        Vimeo,

        /// <summary>
        ///     A stream from Twitch.
        /// </summary>
        Twitch,

        /// <summary>
        ///     A stream from a local file.
        /// </summary>
        Local,

        /// <summary>
        ///     A stream from a HTTP URL (mp3, flac, wav, WebM, MP4/M4A, OGG, AAC, M3U or PLS).
        /// </summary>
        Http,
    }
}