namespace Lavalink4NET.Protocol.Models.RoutePlanners;

using System.Text.Json.Serialization;

public sealed record class RoutePlannerInformationModel(
    [property: JsonRequired]
    [property: JsonPropertyName("class")]
    RoutePlannerType Type,

    [property: JsonRequired]
    [property: JsonPropertyName("details")]
    RoutePlannerDetailsModel Details);

