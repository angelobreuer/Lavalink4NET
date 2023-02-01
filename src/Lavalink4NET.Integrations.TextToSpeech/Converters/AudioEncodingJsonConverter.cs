namespace Lavalink4NET.Integrations.TextToSpeech.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.TextToSpeech.Synthesis;

internal sealed class AudioEncodingJsonConverter : JsonConverter<AudioEncoding>
{
    public override AudioEncoding Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()!;

        return value.ToUpperInvariant() switch
        {
            "AUDIO_ENCODING_UNSPECIFIED" => AudioEncoding.Unspecified,
            "LINEAR16" => AudioEncoding.Linear16Bit,
            "MP3" => AudioEncoding.Mp3,
            "OGG_OPUS" => AudioEncoding.OggOpus,
            "MULAW" => AudioEncoding.Mulaw,
            "ALAW" => AudioEncoding.Alaw,
            _ => throw new JsonException(),
        };
    }

    public override void Write(Utf8JsonWriter writer, AudioEncoding value, JsonSerializerOptions options)
    {
        var serializedValue = value switch
        {
            AudioEncoding.Unspecified => "AUDIO_ENCODING_UNSPECIFIED",
            AudioEncoding.Linear16Bit => "LINEAR16",
            AudioEncoding.Mp3 => "MP3",
            AudioEncoding.OggOpus => "OGG_OPUS",
            AudioEncoding.Mulaw => "MULAW",
            AudioEncoding.Alaw => "ALAW",
            _ => throw new JsonException(),
        };

        writer.WriteStringValue(serializedValue);
    }
}