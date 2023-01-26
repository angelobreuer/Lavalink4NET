namespace Lavalink4NET.Protocol.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class NullableDurationJsonConverter : JsonConverter<TimeSpan?>
{
    public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        var value = reader.GetInt64();
        return value < 0 ? null : TimeSpan.FromMilliseconds(value);
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);

        if (value is null)
        {
            writer.WriteNumberValue(-1);
        }
        else
        {
            if (value.Value < TimeSpan.Zero)
            {
                ThrowNegative();
            }

            writer.WriteNumberValue((long)Math.Round(value.Value.TotalMilliseconds));
        }

        void ThrowNegative() => throw new ArgumentOutOfRangeException(nameof(value), value, "The duration must not be negative.");
    }
}
