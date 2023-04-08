namespace Lavalink4NET.Protocol.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Models.RoutePlanners;

internal sealed class RoutePlannerTypeJsonConverter : JsonConverter<RoutePlannerType>
{
    public override RoutePlannerType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        var value = reader.GetString()!;

        return value.ToUpperInvariant() switch
        {
            "RotatingIpRoutePlanner" => RoutePlannerType.RotatingIpRoutePlanner,
            "NanoIpRoutePlanner" => RoutePlannerType.NanoIpRoutePlanner,
            "RotatingNanoIpRoutePlanner" => RoutePlannerType.RotatingNanoIpRoutePlanner,
            "BalancingIpRoutePlanner" => RoutePlannerType.BalancingIpRoutePlanner,
            _ => throw new JsonException("Invalid route planner type."),
        };
    }

    public override void Write(Utf8JsonWriter writer, RoutePlannerType value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);

        var strValue = value switch
        {
            RoutePlannerType.RotatingIpRoutePlanner => "RotatingIpRoutePlanner",
            RoutePlannerType.NanoIpRoutePlanner => "NanoIpRoutePlanner",
            RoutePlannerType.RotatingNanoIpRoutePlanner => "RotatingNanoIpRoutePlanner",
            RoutePlannerType.BalancingIpRoutePlanner => "BalancingIpRoutePlanner",
            _ => throw new JsonException("Invalid route planner type."),
        };

        writer.WriteStringValue(strValue);
    }
}
