namespace Lavalink4NET.SponsorBlock.Converters;

using Lavalink4NET.Converters;

internal sealed class SegmentCategoryJsonConverter : StaticJsonStringEnumConverter<SegmentCategory>
{
    /// <inheritdoc/>
    protected override void RegisterMappings(RegistrationContext registrationContext)
    {
        registrationContext.Register("sponsor", SegmentCategory.Sponsor);
        registrationContext.Register("selfpromo", SegmentCategory.SelfPromotion);
        registrationContext.Register("interaction", SegmentCategory.Interaction);
        registrationContext.Register("intro", SegmentCategory.Intro);
        registrationContext.Register("outro", SegmentCategory.Outro);
        registrationContext.Register("preview", SegmentCategory.Preview);
        registrationContext.Register("music_offtopic", SegmentCategory.OfftopicMusic);
        registrationContext.Register("filler", SegmentCategory.Filler);
    }
}
