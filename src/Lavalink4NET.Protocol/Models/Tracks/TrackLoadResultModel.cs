namespace Lavalink4NET.Protocol.Responses;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Models;

public sealed record class TrackLoadResultModel(
    [property: JsonRequired]
    [property: JsonPropertyName("data")]
    TrackModel Data) : LoadResultModel;

