namespace Lavalink4NET.Protocol.Responses;

using System.Text.Json.Serialization;

public sealed record class PlaylistLoadResultModel(
    [property: JsonRequired]
    [property: JsonPropertyName("data")]
    PlaylistLoadResultData Data) : LoadResultModel;

