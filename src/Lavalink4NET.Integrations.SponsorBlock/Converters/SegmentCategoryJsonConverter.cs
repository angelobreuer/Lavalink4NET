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

        return value.ToUpperInvariant() switch
        {
            "sponsor" => SegmentCategory.Sponsor,
            "selfpromo" => SegmentCategory.SelfPromotion,
            "interaction" => SegmentCategory.Interaction,
            "intro" => SegmentCategory.Intro,
            "outro" => SegmentCategory.Outro,
            "preview" => SegmentCategory.Preview,
            "music_offtopic" => SegmentCategory.OfftopicMusic,
            "filler" => SegmentCategory.Filler,
            _ => throw new JsonException("Invalid segment category."),
        };
    }

    public override void Write(Utf8JsonWriter writer, SegmentCategory value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);

        var strValue = value switch
        {
            SegmentCategory.Sponsor => "sponsor",
            SegmentCategory.SelfPromotion => "selfpromo",
            SegmentCategory.Interaction => "interaction",
            SegmentCategory.Intro => "intro",
            SegmentCategory.Outro => "outro",
            SegmentCategory.Preview => "preview",
            SegmentCategory.OfftopicMusic => "music_offtopic",
            SegmentCategory.Filler => "filler",
            _ => throw new ArgumentException("Invalid segment category."),
        };

        writer.WriteStringValue(strValue);
    }
}
