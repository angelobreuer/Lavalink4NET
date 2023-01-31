namespace Lavalink4NET.Rest;

/// <summary>
///     Different search modes for the "/tracks" endpoint.
/// </summary>
public enum SearchMode
{
    /// <summary>
    ///     Accepts raw queries.
    /// </summary>
    None,

    /// <summary>
    ///     Only searches for links and YouTube videos.
    /// </summary>
    YouTube,

    /// <summary>
    ///     Only searches for links and SoundCloud tracks.
    /// </summary>
    SoundCloud
}
