/*
 *  File:   LogMessageEventArgs.cs
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

namespace Lavalink4NET.Logging
{
    using System;

    /// <summary>
    ///     The event arguments for the <see cref="EventLogger.LogMessage"/> event.
    /// </summary>
    public sealed class LogMessageEventArgs : EventArgs
    {
        /// <summary>
        ///Gets the source the message comes from.
        /// </summary>
        public object Source { get; }

        /// <summary>
        ///     Gets the message to log.
        /// </summary>
        public string Message { get; }

        /// <summary>
        ///     Gets the logging level / the severity of the message.
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        ///     Gets an optional exception that occurred.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogMessageEventArgs"/> class.
        /// </summary>
        /// <param name="source">the source the message comes from</param>
        /// <param name="message">the message to log</param>
        /// <param name="level">the logging level / the severity of the message</param>
        /// <param name="exception">an optional exception that occurred</param>
        public LogMessageEventArgs(object source, string message, LogLevel level = LogLevel.Information, Exception exception = null)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Level = level;
            Exception = exception;
        }
    }
}