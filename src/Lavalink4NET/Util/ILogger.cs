namespace Lavalink4NET
{
    using System;

    /// <summary>
    ///     An interface for a logger provider.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        ///     Logs a message asynchronously.
        /// </summary>
        /// <param name="source">the source the message comes from (usually this)</param>
        /// <param name="message">the message to log</param>
        /// <param name="level">the logging level / the severity of the message</param>
        /// <param name="exception">an optional exception that occurred</param>
        void Log(object source, string message, LogLevel level = LogLevel.Information, Exception exception = null);
    }
}