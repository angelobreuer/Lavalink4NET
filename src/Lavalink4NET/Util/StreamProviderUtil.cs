/*
 *  File:   StreamProviderUtil.cs
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

namespace Lavalink4NET.Util
{
    using System;
    using Lavalink4NET.Player;

    /// <summary>
    ///     An utility class for detecting the stream provider for a lavaplayer URI.
    /// </summary>
    public sealed class StreamProviderUtil
    {
        /// <summary>
        ///     Gets the stream provider that has the characters for the specified <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">the uri</param>
        /// <returns>the stream provider</returns>
        /// <exception cref="ArgumentException">
        ///     thrown if the specified <paramref name="uri"/> is blank.
        /// </exception>
        public static StreamProvider GetStreamProvider(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                throw new ArgumentException("The specified uri is blank.", nameof(uri));
            }

            if (Uri.TryCreate(uri, UriKind.Absolute, out var result))
            {
                // local file stream
                if (result.IsFile)
                {
                    return StreamProvider.Local;
                }

                // get stream provider
                return GetStreamProvider(result.Host, result.AbsolutePath);
            }

            // uri can not be parsed
            return StreamProvider.Unknown;
        }

        /// <summary>
        ///     Gets a value indicating whether the specified <paramref name="path"/> is a HTTP
        ///     stream URL supported by lavaplayer.
        /// </summary>
        /// <param name="path">the URI path ( <see cref="Uri.AbsolutePath"/>)</param>
        /// <returns>
        ///     a value indicating whether the specified <paramref name="path"/> is a HTTP stream URL
        ///     supported by lavaplayer
        /// </returns>
        public static bool IsHttpStreamUrl(string path)
            => path.EndsWith(".mp3", StringComparison.InvariantCultureIgnoreCase)
            || path.EndsWith(".flac", StringComparison.InvariantCultureIgnoreCase)
            || path.EndsWith(".wav", StringComparison.InvariantCultureIgnoreCase)
            || path.EndsWith(".webm", StringComparison.InvariantCultureIgnoreCase)
            || path.EndsWith(".mp4", StringComparison.InvariantCultureIgnoreCase)
            || path.EndsWith(".m4a", StringComparison.InvariantCultureIgnoreCase)
            || path.EndsWith(".ogg", StringComparison.InvariantCultureIgnoreCase)
            || path.EndsWith(".3gp", StringComparison.InvariantCultureIgnoreCase)
            || path.EndsWith(".mpg", StringComparison.InvariantCultureIgnoreCase)
            || path.EndsWith(".mpeg", StringComparison.InvariantCultureIgnoreCase)
            || path.EndsWith(".m4b", StringComparison.InvariantCultureIgnoreCase)
            || path.EndsWith(".m3u", StringComparison.InvariantCultureIgnoreCase)
            || path.EndsWith(".pls", StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        ///     Gets the stream provider that has the characters for the specified
        ///     <paramref name="host"/> and <paramref name="path"/>.
        /// </summary>
        /// <param name="host">the host (e.g. www.youtube.com)</param>
        /// <param name="path">the watch (e.g. /watch?v=[...])</param>
        /// <returns>the stream provider</returns>
        /// <exception cref="ArgumentException">
        ///     thrown if the specified <paramref name="host"/> is blank.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="path"/> is <see langword="null"/>.
        /// </exception>
        public static StreamProvider GetStreamProvider(string host, string path)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException("Host can not be blank.", nameof(host));
            }

            if (path is null)
            {
                throw new ArgumentNullException("Path can not be null.", nameof(path));
            }

            // YouTube
            if (host.StartsWith("www.youtube.", StringComparison.InvariantCultureIgnoreCase)
                || host.StartsWith("www.youtu.be", StringComparison.InvariantCultureIgnoreCase))
            {
                return StreamProvider.YouTube;
            }

            // Bandcamp
            if (host.Equals("www.bandcamp.com", StringComparison.InvariantCultureIgnoreCase))
            {
                return StreamProvider.Bandcamp;
            }

            // SoundCloud
            if (host.Equals("www.soundcloud.com", StringComparison.InvariantCultureIgnoreCase))
            {
                return StreamProvider.SoundCloud;
            }

            // Vimeo
            if (host.Equals("www.vimeo.com", StringComparison.InvariantCultureIgnoreCase))
            {
                return StreamProvider.Vimeo;
            }

            // Twitch
            if (host.Equals("www.twitch.tv", StringComparison.InvariantCultureIgnoreCase))
            {
                return StreamProvider.Twitch;
            }

            // .mp3, .flac, .wav, .webm, .mp4/.m4a, .ogg, .3gb/.mpg/.mpeg/.m4b, m3u/pls external sources.
            if (IsHttpStreamUrl(path))
            {
                return StreamProvider.Http;
            }

            // unknown
            return StreamProvider.Unknown;
        }
    }
}