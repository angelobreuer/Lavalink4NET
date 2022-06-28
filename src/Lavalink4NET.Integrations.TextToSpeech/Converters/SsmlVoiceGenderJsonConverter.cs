namespace Lavalink4NET.Integrations.TextToSpeech.Converters;

using Lavalink4NET.Converters;
using Lavalink4NET.Integrations.TextToSpeech.Synthesis;

internal sealed class SsmlVoiceGenderJsonConverter : StaticJsonStringEnumConverter<SsmlVoiceGender>
{
    /// <inheritdoc/>
    protected override void RegisterMappings(RegistrationContext registrationContext)
    {
        registrationContext.Register("SSML_VOICE_GENDER_UNSPECIFIED", SsmlVoiceGender.Unspecified);
        registrationContext.Register("MALE", SsmlVoiceGender.Male);
        registrationContext.Register("FEMALE", SsmlVoiceGender.Female);
        registrationContext.Register("NEUTRAL", SsmlVoiceGender.Neutral);
    }
}