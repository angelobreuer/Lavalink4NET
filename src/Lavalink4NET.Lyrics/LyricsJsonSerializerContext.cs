namespace Lavalink4NET.Lyrics;

using System.Text.Json.Serialization;
using Lavalink4NET.Lyrics.Models;

[JsonSerializable(typeof(LyricsResponse))]
internal sealed partial class LyricsJsonSerializerContext : JsonSerializerContext
{
}
