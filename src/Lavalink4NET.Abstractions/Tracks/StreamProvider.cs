namespace Lavalink4NET.Tracks;

/// <summary>
///     A set of different stream providers supported by lavaplayer (https://github.com/sedmelluq/lavaplayer).
/// </summary>
public enum StreamProvider : byte
{
    /// <summary>
    ///     A stream from YouTube.
    /// </summary>
    YouTube,

    /// <summary>
    ///     A stream from SoundCloud.
    /// </summary>
    SoundCloud,

    /// <summary>
    ///     A stream from Bandcamp.
    /// </summary>
    Bandcamp,

    /// <summary>
    ///     A stream from Vimeo.
    /// </summary>
    Vimeo,

    /// <summary>
    ///     A stream from getyarn.io.
    /// </summary>
    GetYarn,

    /// <summary>
    ///     A stream from Twitch.
    /// </summary>
    Twitch,

    /// <summary>
    ///     A stream from niconico.
    /// </summary>
    NicoNico,

    /// <summary>
    ///     A stream from beam.pro.
    /// </summary>
    Beam,

    /// <summary>
    ///     A stream from a local file.
    /// </summary>
    Local,

    /// <summary>
    ///     A stream from a HTTP URL (mp3, flac, wav, WebM, MP4/M4A, OGG, AAC, M3U or PLS).
    /// </summary>
    Http,
}
