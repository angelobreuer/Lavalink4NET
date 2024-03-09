namespace Lavalink4NET.Integrations.LyricsJava;

public readonly record struct TimedLyricsLine(
    string Line,
    TimeRange Range);