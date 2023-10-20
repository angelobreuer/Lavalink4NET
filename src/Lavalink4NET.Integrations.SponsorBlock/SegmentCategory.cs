namespace Lavalink4NET.Integrations.SponsorBlock;

using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.SponsorBlock.Converters;

[JsonConverter(typeof(SegmentCategoryJsonConverter))]
public readonly record struct SegmentCategory(string Value)
{
    public static SegmentCategory Sponsor { get; } = new("sponsor");

    public static SegmentCategory SelfPromotion { get; } = new("selfpromo");

    public static SegmentCategory Interaction { get; } = new("interaction");

    public static SegmentCategory Intro { get; } = new("intro");

    public static SegmentCategory Outro { get; } = new("outro");

    public static SegmentCategory Preview { get; } = new("preview");

    public static SegmentCategory OfftopicMusic { get; } = new("music_offtopic");

    public static SegmentCategory Filler { get; } = new("filler");
}
