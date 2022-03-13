namespace Lavalink4NET.Logging.Microsoft;

using System;
using System.Runtime.CompilerServices;
using global::Microsoft.Extensions.Logging;

internal sealed class MicrosoftExtensionsLogger : Logging.ILogger
{
    private readonly ConditionalWeakTable<object, ILogger> _loggerCache;
    private readonly ILoggerFactory _loggerFactory;

    public MicrosoftExtensionsLogger(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _loggerCache = new ConditionalWeakTable<object, ILogger>();
    }

    /// <inheritdoc/>
    public void Log(object source, string message, Logging.LogLevel level = Logging.LogLevel.Information, Exception? exception = null)
    {
        var logger = _loggerCache.GetValue(source, source => _loggerFactory.CreateLogger(source.GetType()));

        var logLevel = level switch
        {
            Logging.LogLevel.Information => LogLevel.Information,
            Logging.LogLevel.Error => LogLevel.Error,
            Logging.LogLevel.Warning => LogLevel.Warning,
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
