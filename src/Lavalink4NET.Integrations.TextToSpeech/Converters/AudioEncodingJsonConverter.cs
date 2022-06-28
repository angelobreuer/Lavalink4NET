namespace Lavalink4NET.Integrations.TextToSpeech.Converters;

using Lavalink4NET.Converters;
using Lavalink4NET.Integrations.TextToSpeech.Synthesis;

internal sealed class AudioEncodingJsonConverter : StaticJsonStringEnumConverter<AudioEncoding>
{
    /// <inheritdoc/>
    protected override void RegisterMappings(RegistrationContext registrationContext)
    {
        registrationContext.Register("AUDIO_ENCODING_UNSPECIFIED", AudioEncoding.Unspecified);
        registrationContext.Register("LINEAR16", AudioEncoding.Linear16Bit);
        registrationContext.Register("MP3", AudioEncoding.Mp3);
        registrationContext.Register("OGG_OPUS", AudioEncoding.OggOpus);
        registrationContext.Register("MULAW", AudioEncoding.Mulaw);
        registrationContext.Register("ALAW", AudioEncoding.Alaw);
    }
}