namespace Lavalink4NET.Integrations.LyricsJava.Converters;

using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class NumberTimeSpanJsonConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return TimeSpan.FromMilliseconds(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((long)value.TotalMilliseconds);
    }
}
