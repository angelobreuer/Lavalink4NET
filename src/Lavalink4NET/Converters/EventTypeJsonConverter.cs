namespace Lavalink4NET.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lavalink4NET.Payloads;

internal sealed class EventTypeJsonConverter : JsonConverter<EventType>
{
    /// <inheritdoc/>
    public override EventType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new EventType(reader.GetString()!);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, EventType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
