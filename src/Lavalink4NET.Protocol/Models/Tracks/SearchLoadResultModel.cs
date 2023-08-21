namespace Lavalink4NET.Protocol.Responses;

using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Models;

public sealed record class SearchLoadResultModel(
    [property: JsonRequired]
    [property: JsonPropertyName("data")]
    ImmutableArray<TrackModel> Data) : LoadResultModel;

