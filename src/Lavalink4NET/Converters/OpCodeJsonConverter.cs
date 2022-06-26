namespace Lavalink4NET.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lavalink4NET.Payloads;

internal sealed class OpCodeJsonConverter : JsonConverter<OpCode>
{
    /// <inheritdoc/>
    public override OpCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new OpCode(reader.GetString()!);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, OpCode value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
