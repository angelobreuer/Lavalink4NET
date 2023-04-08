namespace Lavalink4NET.Protocol.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Models.RoutePlanners;

internal sealed class RoutePlannerIpBlockTypeJsonConverter : JsonConverter<RoutePlannerIpBlockType>
{
    public override RoutePlannerIpBlockType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        var value = reader.GetString()!;

        return value switch
        {
            "Inet4Address" => RoutePlannerIpBlockType.Inet4Address,
            "Inet6Address" => RoutePlannerIpBlockType.Inet6Address,
            _ => throw new JsonException("Invalid ip block type."),
        };
    }

    public override void Write(Utf8JsonWriter writer, RoutePlannerIpBlockType value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);

        var strValue = value switch
        {
            RoutePlannerIpBlockType.Inet4Address => "Inet4Address",
            RoutePlannerIpBlockType.Inet6Address => "Inet6Address",
            _ => throw new JsonException("Invalid ip block type."),
        };

        writer.WriteStringValue(strValue);
    }
}
