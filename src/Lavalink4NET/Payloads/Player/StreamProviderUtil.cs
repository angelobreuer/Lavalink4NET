namespace Lavalink4NET.Payloads.Player;

using System;
using Lavalink4NET.Player;

/// <summary>
///     An utility class for detecting the stream provider for a lavaplayer URI.
/// </summary>
public static class StreamProviderUtil
{
    /// <summary>
    ///     Gets the stream provider that has the characters for the specified <paramref name="uri"/>.
    /// </summary>
    /// <param name="uri">the uri</param>
    /// <returns>the stream provider</returns>
    /// <exception cref="ArgumentException">
    ///     thrown if the specified <paramref name="uri"/> is blank.
    /// </exception>
    public static StreamProvider GetStreamProvider(Uri? uri)
    {
        if (uri is null)
        {
            return StreamProvider.Unknown;
        }

        // local file stream
        if (uri.IsFile)
        {
            return StreamProvider.Local;
        }

        // get stream provider
        return GetStreamProvider(uri.Host, uri.AbsolutePath);
    }

    public static StreamProvider GetStreamProvider(string sourceName) => sourceName?.ToUpperInvariant() switch
    {
        "SOUNDCLOUD" => StreamProvider.SoundCloud,
        "YOUTUBE" => StreamProvider.YouTube,
        "BANDCAMP" => StreamProvider.Bandcamp,
        "TWITCH" => StreamProvider.Twitch,
        "HTTP" => StreamProvider.Http,
        "NICONICO" => StreamProvider.NicoNico,
        "VIMEO" => StreamProvider.Vimeo,
        "GETYARN.IO" => StreamProvider.GetYarn,
        "LOCAL" => StreamProvider.Local,
        "BEAM.PRO" => StreamProvider.Beam,
        _ => StreamProvider.Unknown,
    };

    /// <summary>
    ///     Gets a value indicating whether the specified <paramref name="path"/> is a HTTP
    ///     stream URL supported by lavaplayer.
    /// </summary>
    /// <param name="path">the URI path ( <see cref="Uri.AbsolutePath"/>)</param>
    /// <returns>
    ///     a value indicating whether the specified <paramref name="path"/> is a HTTP stream URL
    ///     supported by lavaplayer
    /// </returns>
    public static bool IsHttpStreamUrl(string path)
        => path.EndsWith(".mp3", StringComparison.InvariantCultureIgnoreCase)
        || path.EndsWith(".flac", StringComparison.InvariantCultureIgnoreCase)
        || path.EndsWith(".wav", StringComparison.InvariantCultureIgnoreCase)
        || path.EndsWith(".webm", StringComparison.InvariantCultureIgnoreCase)
        || path.EndsWith(".mp4", StringComparison.InvariantCultureIgnoreCase)
        || path.EndsWith(".m4a", StringComparison.InvariantCultureIgnoreCase)
        || path.EndsWith(".ogg", StringComparison.InvariantCultureIgnoreCase)
        || path.EndsWith(".3gp", StringComparison.InvariantCultureIgnoreCase)
        || path.EndsWith(".mpg", StringComparison.InvariantCultureIgnoreCase)
        || path.EndsWith(".mpeg", StringComparison.InvariantCultureIgnoreCase)
        || path.EndsWith(".m4b", StringComparison.InvariantCultureIgnoreCase)
        || path.EndsWith(".m3u", StringComparison.InvariantCultureIgnoreCase)
        || path.EndsWith(".pls", StringComparison.InvariantCultureIgnoreCase);

    /// <summary>
    ///     Gets the stream provider that has the characters for the specified
    ///     <paramref name="host"/> and <paramref name="path"/>.
    /// </summary>
    /// <param name="host">the host (e.g. www.youtube.com)</param>
    /// <param name="path">the watch (e.g. /watch?v=[...])</param>
    /// <returns>the stream provider</returns>
    /// <exception cref="ArgumentException">
    ///     thrown if the specified <paramref name="host"/> is blank.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    public static StreamProvider GetStreamProvider(string host, string path)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            throw new ArgumentException("Host can not be blank.", nameof(host));
        }

        if (path is null)
        {
            throw new ArgumentNullException("Path can not be null.", nameof(path));
        }

        // YouTube
        if (host.StartsWith("www.youtube.", StringComparison.InvariantCultureIgnoreCase)
            || host.StartsWith("www.youtu.be", StringComparison.InvariantCultureIgnoreCase))
        {
            return StreamProvider.YouTube;
        }

        // Bandcamp
        if (host.Equals("www.bandcamp.com", StringComparison.InvariantCultureIgnoreCase))
        {
            return StreamProvider.Bandcamp;
        }

        // SoundCloud
        if (host.Equals("www.soundcloud.com", StringComparison.InvariantCultureIgnoreCase))
        {
            return StreamProvider.SoundCloud;
        }

        // Vimeo
        if (host.Equals("www.vimeo.com", StringComparison.InvariantCultureIgnoreCase))
        {
            return StreamProvider.Vimeo;
        }

        // Twitch
        if (host.Equals("www.twitch.tv", StringComparison.InvariantCultureIgnoreCase))
        {
            return StreamProvider.Twitch;
        }

        // GetYarn.io
        if (host.Equals("www.getyarn.io", StringComparison.InvariantCultureIgnoreCase))
        {
            return StreamProvider.GetYarn;
        }

        // nicovideo.jp
        if (host.Equals("www.nicovideo.jp", StringComparison.InvariantCultureIgnoreCase))
        {
            return StreamProvider.NicoNico;
        }

        // .mp3, .flac, .wav, .webm, .mp4/.m4a, .ogg, .3gb/.mpg/.mpeg/.m4b, m3u/pls external sources.
        if (IsHttpStreamUrl(path))
        {
            return StreamProvider.Http;
        }

        // unknown
        return StreamProvider.Unknown;
    }
}
