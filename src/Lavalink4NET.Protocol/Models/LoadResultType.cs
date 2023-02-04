namespace Lavalink4NET.Protocol.Models;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

[JsonConverter(typeof(LoadResultTypeJsonConverter))]
public enum LoadResultType : byte
{
    TrackLoaded,
    PlaylistLoaded,
    SearchResult,
    NoMatches,
    LoadFailed,
}
