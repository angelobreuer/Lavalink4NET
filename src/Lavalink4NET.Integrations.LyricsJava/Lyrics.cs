namespace Lavalink4NET.Integrations.LyricsJava;

using System.Collections.Immutable;

public sealed record class Lyrics(
    string Source,
    string Text,
    LyricsTrack Track,
    ImmutableArray<TimedLyricsLine>? TimedLines);