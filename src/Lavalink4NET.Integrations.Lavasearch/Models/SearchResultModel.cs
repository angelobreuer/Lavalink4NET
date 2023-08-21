namespace Lavalink4NET.Integrations.Lavasearch.Models;

using System.Collections.Immutable;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Protocol.Responses;

public sealed record class SearchResultModel
{
    [JsonPropertyName("tracks")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ImmutableArray<TrackModel>? Tracks { get; set; }

    [JsonPropertyName("albums")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ImmutableArray<PlaylistLoadResultData>? Albums { get; set; }

    [JsonPropertyName("artists")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ImmutableArray<PlaylistLoadResultData>? Artists { get; set; }

    [JsonPropertyName("playlists")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ImmutableArray<PlaylistLoadResultData>? Playlists { get; set; }

    [JsonPropertyName("texts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ImmutableArray<TextResultModel>? Texts { get; set; }

    [JsonRequired]
    [JsonPropertyName("plugin")]
    public IImmutableDictionary<string, JsonNode> AdditionalInformation { get; set; } = null!;
}
