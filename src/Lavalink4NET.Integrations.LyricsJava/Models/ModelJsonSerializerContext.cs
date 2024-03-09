namespace Lavalink4NET.Integrations.LyricsJava.Models;

using System.Collections.Immutable;
using System.Text.Json.Serialization;

[JsonSerializable(typeof(AlbumArtModel))]
[JsonSerializable(typeof(LyricsResponseModel))]
[JsonSerializable(typeof(ImmutableArray<SearchResultModel>))]
[JsonSerializable(typeof(LyricsResponseTrackModel))]
[JsonSerializable(typeof(SearchResultModel))]
[JsonSerializable(typeof(TimedLyricsLineModel))]
[JsonSerializable(typeof(TimeRangeModel))]
internal sealed partial class ModelJsonSerializerContext : JsonSerializerContext
{
}
