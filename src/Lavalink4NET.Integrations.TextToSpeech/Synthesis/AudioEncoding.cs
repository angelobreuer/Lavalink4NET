namespace Lavalink4NET.Integrations.TextToSpeech.Synthesis;

using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.TextToSpeech.Converters;

[JsonConverter(typeof(AudioEncodingJsonConverter))]
public enum AudioEncoding
{
    Unspecified,
    Linear16Bit,
    Mp3,
    OggOpus,
    Mulaw,
    Alaw,
}