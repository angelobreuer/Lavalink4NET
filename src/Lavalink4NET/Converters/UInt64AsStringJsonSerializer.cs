namespace Lavalink4NET.Converters;
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class UInt64AsStringJsonSerializer : JsonConverter<ulong>
{
    /// <inheritdoc/>
    public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ulong.Parse(reader.GetString()!);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
    }
}
