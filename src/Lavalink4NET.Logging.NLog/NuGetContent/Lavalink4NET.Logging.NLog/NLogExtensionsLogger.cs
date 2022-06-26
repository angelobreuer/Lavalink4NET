/*
 *  File:   NLogExtensionsLogger.cs
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

namespace Lavalink4NET.Logging.NLog;

using System;
using System.Runtime.CompilerServices;
using global::NLog;

internal sealed class NLogExtensionsLogger : Logging.ILogger
{
    private readonly ConditionalWeakTable<object, ILogger> _loggerCache;
    private readonly LogFactory _logFactory;

    public NLogExtensionsLogger(LogFactory logFactory)
    {
        _logFactory = logFactory ?? throw new ArgumentNullException(nameof(logFactory));
        _loggerCache = new ConditionalWeakTable<object, ILogger>();
    }

    /// <inheritdoc/>
    public void Log(object source, string message, Logging.LogLevel level = Logging.LogLevel.Information, Exception? exception = null)
    {
        var logger = _loggerCache.GetValue(source, source => _logFactory.GetCurrentClassLogger(source.GetType()));

        var logLevel = level switch
        {
            Logging.LogLevel.Information => LogLevel.Info,
            Logging.LogLevel.Error => LogLevel.Error,
            Logging.LogLevel.Warning => LogLevel.Warn,
            Logging.LogLevel.Debug => LogLevel.Debug,
            Logging.LogLevel.Trace => LogLevel.Trace,

            _ => throw new ArgumentOutOfRangeException(
                paramName: nameof(level),
                actualValue: level,
                message: "The specified log level does not exist."),
        };

        logger.Log(logLevel, exception, message, args: Array.Empty<object>());
    }
}
