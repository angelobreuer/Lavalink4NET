namespace Lavalink4NET.Protocol.Models.RoutePlanners;

using System.Text.Json.Serialization;

public sealed record class RoutePlannerIpBlockModel(
    [property: JsonRequired]
    [property: JsonPropertyName("type")]
    RoutePlannerIpBlockType Type,

    [property: JsonRequired]
    [property: JsonPropertyName("size")]
    string Size);

