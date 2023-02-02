namespace Lavalink4NET.Protocol.Models;

public enum LoadResultType : byte
{
    TrackLoaded,
    PlaylistLoaded,
    SearchResult,
    NoMatches,
    LoadFailed,
}
