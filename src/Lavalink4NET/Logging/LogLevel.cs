/*
 *  File:   LogLevel.cs
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

namespace Lavalink4NET.Logging
{
    /// <summary>
    ///     A set of different logging levels.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        ///     Information, not critical, just for user information.
        /// </summary>
        Information,

        /// <summary>
        ///     Error, critical (application can continue)
        /// </summary>
        Error,

        /// <summary>
        ///     Warning, not critical (a warning, but the application can continue without any
        ///     further problems)
        /// </summary>
        Warning,

        /// <summary>
        ///     Debug message, not critical (just for information / debugging)
        /// </summary>
        Debug,

        /// <summary>
        ///     Trace message, not critical (just for information / debugging)
        /// </summary>
        Trace
    }
}