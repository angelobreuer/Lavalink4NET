namespace Lavalink4NET
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