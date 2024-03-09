namespace Lavalink4NET.Integrations.LyricsJava.Converters;

using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class StringUriJsonConverter : JsonConverter<Uri>
{
    public override Uri Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new Uri(reader.GetString() ?? string.Empty);
    }

    public override void Write(Utf8JsonWriter writer, Uri value, JsonSerializerOptions options)
    {
        writer.WriteString("url", value.ToString());
    }
}
