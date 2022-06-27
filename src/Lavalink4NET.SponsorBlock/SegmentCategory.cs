namespace Lavalink4NET.SponsorBlock;

using System.Text.Json.Serialization;
using Lavalink4NET.SponsorBlock.Converters;

[JsonConverter(typeof(SegmentCategoryJsonConverter))]
public enum SegmentCategory : byte
{
    Sponsor,
    SelfPromotion,
    Interaction,
    Intro,
    Outro,
    Preview,
    OfftopicMusic,
    Filler,
}
