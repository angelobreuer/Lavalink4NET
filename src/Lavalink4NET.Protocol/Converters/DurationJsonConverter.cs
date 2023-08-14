namespace Lavalink4NET.Protocol.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class DurationJsonConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        var value = reader.GetInt64();

        if (value is long.MaxValue)
        {
            return TimeSpan.MaxValue;
        }

        return TimeSpan.FromMilliseconds(value);
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);

        if (value == TimeSpan.MaxValue)
        {
            writer.WriteNumberValue(long.MaxValue);
            return;
        }

        writer.WriteNumberValue((long)Math.Round(value.TotalMilliseconds));
    }
}
