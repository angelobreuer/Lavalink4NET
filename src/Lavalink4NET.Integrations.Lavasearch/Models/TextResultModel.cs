namespace Lavalink4NET.Integrations.Lavasearch.Models;

using System.Collections.Immutable;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

public sealed record class TextResultModel
{
    [JsonRequired]
    [JsonPropertyName("text")]
    public string Text { get; set; } = null!;

    [JsonRequired]
    [JsonPropertyName("plugin")]
    public IImmutableDictionary<string, JsonNode> AdditionalInformation { get; set; } = null!;
}
