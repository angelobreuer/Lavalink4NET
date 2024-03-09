namespace Lavalink4NET.Integrations.LyricsJava;

using System.Collections.Immutable;

public sealed record class Lyrics(
    LyricsType Type,
    string? Source,
    string? Basic,
    LyricsTrack? Track,
    ImmutableArray<TimedLyricsLine>? Timed 
);