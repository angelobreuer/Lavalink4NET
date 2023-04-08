namespace Lavalink4NET.Protocol.Models.RoutePlanners;

using System;
using System.Text.Json.Serialization;

public sealed record class FailingRoutePlannerAddressModel(
    [property: JsonRequired]
    [property: JsonPropertyName("failingAddress")]
    string Address,

    [property: JsonRequired]
    [property: JsonPropertyName("failingTimestamp")]
    [property: JsonConverter(typeof(UnixTimestampJsonConverter))]
    DateTimeOffset FailingSince);

