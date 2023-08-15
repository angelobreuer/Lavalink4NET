namespace Lavalink4NET.Integrations.Lavasearch.Models;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

public sealed record class TextResultModel
{
    [JsonRequired]
    [JsonPropertyName("text")]
    public string Text { get; set; } = null!;

    [JsonRequired]
    [JsonPropertyName("plugin")]
    public IDictionary<string, JsonNode> AdditionalData { get; set; } = null!;
}
