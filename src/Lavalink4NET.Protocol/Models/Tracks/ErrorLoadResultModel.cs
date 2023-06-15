namespace Lavalink4NET.Protocol.Responses;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Payloads;

public sealed record class ErrorLoadResultModel(
    [property: JsonRequired]
    [property: JsonPropertyName("data")]
    TrackExceptionModel Data) : LoadResultModel;

