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
