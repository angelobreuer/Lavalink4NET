namespace Lavalink4NET.Lyrics;

/// <summary>
///     The service options for the <see cref="LyricsService"/> class.
/// </summary>
public sealed class LyricsOptions
{
    private static readonly Uri _defaultBaseAddress = new("https://api.lyrics.ovh/v1/");

    /// <summary>
    ///     Gets or sets the base endpoint of the Lyrics API service ("lyrics.ovh"). This
    ///     property can be useful when using a local lyrics.ovh API service.
    /// </summary>
    /// <remarks>
    ///     This property defaults to <c>"https://api.lyrics.ovh/v1/"</c>. Note this is an
    ///     absolute URI and can not be <see langword="null"/>.
    /// </remarks>
    public Uri BaseAddress { get; init; } = _defaultBaseAddress;

    /// <summary>
    ///     Gets or sets a value indicating whether an exception should be thrown when a response
    ///     to the lyrics.ovh API service failed (returned with a non-2xx / success HTTP status
    ///     code). (For example the lyrics.ovh API service returns with a 404 Not Found, if the
    ///     lyrics for a song were not found.)
    /// </summary>
    /// <remarks>This property defaults to <see langword="true"/>.</remarks>
    public bool SuppressExceptions { get; init; } = true;

    public TimeSpan CacheDuration { get; init; } = TimeSpan.FromMinutes(30);
}
