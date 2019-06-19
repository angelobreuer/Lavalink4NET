namespace Example.CustomLogger
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     A wrapper for <see cref="ILogger"/> as a <see cref="Lavalink4NET.ILogger"/> that can be
    ///     used for Lavalink4NET logging.
    /// </summary>
    public sealed class CustomLogger : Lavalink4NET.ILogger
    {
        /// <summary>
        ///     The logger factory used to create loggers.
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        ///     THe logger object instantiation cache.
        /// </summary>
        private readonly IDictionary<string, ILogger> _loggers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomLogger"/> class.
        /// </summary>
        /// <param name="loggerFactory">the <see cref="ILoggerFactory"/> used to create loggers</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="loggerFactory"/> is <see langword="null"/>.
        /// </exception>
        public CustomLogger(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

            // Initialize logger object instantiation cache
            _loggers = new Dictionary<string, ILogger>();
        }

        /// <summary>
        ///     Gets a logger instance for the specified <paramref name="categoryName"/>.
        /// </summary>
        /// <param name="categoryName">the category name for the logger</param>
        /// <returns>the logger instance</returns>
        public ILogger GetLogger(string categoryName)
        {
            // ensure that the specified categoryName is not null, empty or only consists of whitespaces.
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("The specified category name can not be blank.", nameof(categoryName));
            }

            // no logger was created for the category name
            if (!_loggers.TryGetValue(categoryName, out var logger))
            {
                // create logger instance
                logger = _loggerFactory.CreateLogger(categoryName);

                // store logger instance
                _loggers.Add(categoryName, logger);
            }

            return logger;
        }

        /// <summary>
        ///     Logs a message asynchronously.
        /// </summary>
        /// <param name="source">the source the message comes from (usually this)</param>
        /// <param name="message">the message to log</param>
        /// <param name="level">the logging level / the severity of the message</param>
        /// <param name="exception">an optional exception that occurred</param>
        public void Log(object source, string message, Lavalink4NET.LogLevel level = Lavalink4NET.LogLevel.Information, Exception exception = null)
        {
            // find the log level equivalent for Microsoft.Extensions.Logging.LogLevel, the
            // Lavalink4NET.LogLevel and Microsoft.Extensions.Logging.LogLevel enumeration field
            // names are the same.
            if (!Enum.TryParse<LogLevel>(level.ToString(), out var logLevel))
            {
                // No log level equivalent found, use LogLevel.Information.
                logLevel = LogLevel.Information;
            }

            // create / get the logger
            var logger = GetLogger(source.GetType().FullName);

            // log the message
            logger.Log(logLevel, exception, message);
        }
    }
}