namespace Lavalink4NET.Integrations.SponsorBlock.Converters;

using Lavalink4NET.Converters;
using Lavalink4NET.Integrations.SponsorBlock;

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
