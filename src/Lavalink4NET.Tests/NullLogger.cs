namespace Lavalink4NET.Tests
{
    using System;

    /// <summary>
    ///     An dummy <see cref="ILogger"/> implementation.
    /// </summary>
    /// <remarks>
    ///     All actions done with this <see cref="ILogger"/> will result in a
    ///     <see cref="NotImplementedException"/> exception.
    /// </remarks>
    internal sealed class NullLogger : ILogger
    {
        /// <summary>
        ///     Logs a message asynchronously.
        /// </summary>
        /// <param name="source">the source the message comes from (usually this)</param>
        /// <param name="message">the message to log</param>
        /// <param name="level">the logging level / the severity of the message</param>
        /// <param name="exception">an optional exception that occurred</param>
        public void Log(object source, string message, LogLevel level = LogLevel.Information, Exception exception = null)
        {
            throw new NotImplementedException();
        }
    }
}