namespace Lavalink4NET.Integrations.LyricsJava;

using System.Collections.Immutable;

public sealed record class LyricsTrack(
    string Title,
    string Author,
    string Album,
    ImmutableArray<AlbumArt> AlbumArt);