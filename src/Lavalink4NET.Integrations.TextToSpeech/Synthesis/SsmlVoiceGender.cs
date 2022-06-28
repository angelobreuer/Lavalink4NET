namespace Lavalink4NET.Integrations.TextToSpeech.Synthesis;

using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.TextToSpeech.Converters;

[JsonConverter(typeof(SsmlVoiceGenderJsonConverter))]
public enum SsmlVoiceGender
{
    Unspecified,
    Male,
    Female,
    Neutral,
}