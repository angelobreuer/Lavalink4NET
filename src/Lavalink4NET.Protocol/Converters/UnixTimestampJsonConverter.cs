namespace Lavalink4NET.Protocol;

using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class UnixTimestampJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        return DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);

        writer.WriteNumberValue(value.ToUnixTimeMilliseconds());
    }
}