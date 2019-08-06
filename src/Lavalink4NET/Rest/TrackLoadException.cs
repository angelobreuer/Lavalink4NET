/*
 *  File:   TrackLoadException.cs
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

namespace Lavalink4NET.Rest
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    ///     An exception for track load exception.
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class TrackLoadException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TrackLoadException"/> class.
        /// </summary>
        /// <param name="friendlyMessage">a localized message from the Lavalink Node</param>
        /// <param name="severity">
        ///     the exception severity; 'COMMON' indicates that the exception is not from Lavalink itself.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="severity"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///     This is a JSON constructor, which should be only used by Json.Net for the
        ///     deserialization of the object.
        /// </remarks>
        [JsonConstructor]
        [Obsolete("This constructor should be only used by Json.Net")]
        public TrackLoadException(string friendlyMessage, string severity)
            : base(friendlyMessage)
        {
            Severity = severity ?? throw new ArgumentNullException(nameof(severity));
        }

        /// <summary>
        ///     Gets the exception severity.
        /// </summary>
        /// <remarks>'COMMON' indicates that the exception is not from Lavalink itself</remarks>
        public string Severity { get; internal set; }
    }
}
