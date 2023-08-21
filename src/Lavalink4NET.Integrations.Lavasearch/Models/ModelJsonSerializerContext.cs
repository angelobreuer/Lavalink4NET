namespace Lavalink4NET.Integrations.Lavasearch.Models;

using System.Text.Json.Serialization;

[JsonSerializable(typeof(SearchResultModel))]
[JsonSerializable(typeof(TextResultModel))]
internal sealed partial class ModelJsonSerializerContext : JsonSerializerContext
{
}
