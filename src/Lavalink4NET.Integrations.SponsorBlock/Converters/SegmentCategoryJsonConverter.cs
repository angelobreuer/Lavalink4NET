namespace Lavalink4NET.Integrations.SponsorBlock.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.SponsorBlock;

internal sealed class SegmentCategoryJsonConverter : JsonConverter<SegmentCategory>
{
    public override SegmentCategory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        var value = reader.GetString()!;
        return new SegmentCategory(value);
    }

    public override void Write(Utf8JsonWriter writer, SegmentCategory value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);

        writer.WriteStringValue(value.Value);
    }
}
