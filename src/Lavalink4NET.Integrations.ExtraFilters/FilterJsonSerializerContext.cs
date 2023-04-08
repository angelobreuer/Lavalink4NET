namespace Lavalink4NET.Integrations.ExtraFilters;

using System.Text.Json.Serialization;

[JsonSerializable(typeof(EchoFilterModel))]
internal sealed partial class FilterJsonSerializerContext : JsonSerializerContext
{
}
