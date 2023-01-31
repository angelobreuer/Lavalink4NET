namespace Lavalink4NET.Tracks;

internal static class StreamHeuristics
{
    public static StreamProvider? GetStreamProvider(string? sourceName) => sourceName?.ToUpperInvariant() switch
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
        _ => null,
    };
}
