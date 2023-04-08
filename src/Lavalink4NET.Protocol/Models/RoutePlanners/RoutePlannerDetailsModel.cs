namespace Lavalink4NET.Protocol.Models.RoutePlanners;

using System.Collections.Immutable;
using System.Text.Json.Serialization;

public sealed record class RoutePlannerDetailsModel(
    [property: JsonRequired]
    [property: JsonPropertyName("ipBlock")]
    RoutePlannerIpBlockModel IpBlock,

    [property: JsonRequired]
    [property: JsonPropertyName("failingAddresses")]
    ImmutableArray<FailingRoutePlannerAddressModel> FailingAddresses,

    [property: JsonPropertyName("rotateIndex")]
    string? RotateIndex,

    [property: JsonPropertyName("ipIndex")]
    string? IpIndex,

    [property: JsonPropertyName("currentAddress")]
    string? CurrentAddress,

    [property: JsonPropertyName("currentAddressIndex")]
    string? CurrentAddressIndex,

    [property: JsonPropertyName("blockIndex")]
    string? BlockIndex);

