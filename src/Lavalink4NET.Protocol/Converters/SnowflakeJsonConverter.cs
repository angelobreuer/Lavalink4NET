namespace Lavalink4NET.Protocol.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class SnowflakeJsonConverter : JsonConverter<ulong>
{
    public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        if (reader.TokenType is JsonTokenType.String)
        {
            return ulong.Parse(reader.GetString()!);
        }
        else
        {
            return reader.GetUInt64();
        }
    }

    public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);

        // Lavalink/Lavaplayer seems to prefer snowflakes serialized as strings
        writer.WriteStringValue(value.ToString());
    }
}
