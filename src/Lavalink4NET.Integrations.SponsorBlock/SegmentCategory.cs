namespace Lavalink4NET.Integrations.SponsorBlock;

using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.SponsorBlock.Converters;

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
