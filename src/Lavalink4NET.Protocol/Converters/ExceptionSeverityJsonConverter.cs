namespace Lavalink4NET.Protocol.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Models;

internal sealed class ExceptionSeverityJsonConverter : JsonConverter<ExceptionSeverity>
{
    public override ExceptionSeverity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        var value = reader.GetString()!;

        return value.ToUpperInvariant() switch
        {
            "COMMON" => ExceptionSeverity.Common,
            "SUSPICIOUS" => ExceptionSeverity.Suspicious,
            "FATAL" => ExceptionSeverity.Fatal,
            _ => throw new JsonException("Invalid severity."),
        };
    }

    public override void Write(Utf8JsonWriter writer, ExceptionSeverity value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);

        var strValue = value switch
        {
            ExceptionSeverity.Common => "COMMON",
            ExceptionSeverity.Suspicious => "SUSPICIOUS",
            ExceptionSeverity.Fatal => "FATAL",
            _ => throw new ArgumentException("Invalid severity."),
        };

        writer.WriteStringValue(strValue);
    }
}
