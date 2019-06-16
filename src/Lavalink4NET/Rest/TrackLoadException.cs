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
        ///     thrown if the specified <paramref name="friendlyMessage"/> is <see langword="null"/>.
        /// </exception>
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
            if (friendlyMessage is null)
            {
                throw new ArgumentNullException(nameof(friendlyMessage));
            }

            Severity = severity ?? throw new ArgumentNullException(nameof(severity));
        }

        /// <summary>
        ///     Gets the exception severity.
        /// </summary>
        /// <remarks>'COMMON' indicates that the exception is not from Lavalink itself</remarks>
        public string Severity { get; internal set; }
    }
}