namespace Lavalink4NET.Integrations.TextToSpeech.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.TextToSpeech.Synthesis;

internal sealed class SsmlVoiceGenderJsonConverter : JsonConverter<SsmlVoiceGender>
{
    public override SsmlVoiceGender Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()!;

        return value.ToUpperInvariant() switch
        {
            "SSML_VOICE_GENDER_UNSPECIFIED" => SsmlVoiceGender.Unspecified,
            "MALE" => SsmlVoiceGender.Male,
            "FEMALE" => SsmlVoiceGender.Female,
            "NEUTRAL" => SsmlVoiceGender.Neutral,
            _ => throw new JsonException(),
        };
    }

    public override void Write(Utf8JsonWriter writer, SsmlVoiceGender value, JsonSerializerOptions options)
    {
        var serializedValue = value switch
        {
            SsmlVoiceGender.Unspecified => "SSML_VOICE_GENDER_UNSPECIFIED",
            SsmlVoiceGender.Male => "MALE",
            SsmlVoiceGender.Female => "FEMALE",
            SsmlVoiceGender.Neutral => "NEUTRAL",
            _ => throw new JsonException(),
        };

        writer.WriteStringValue(serializedValue);
    }
}