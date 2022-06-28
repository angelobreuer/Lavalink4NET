namespace Lavalink4NET.Integrations.TextToSpeech;

using System.Text;
using System.Text.Json;
using System.Web;
using Lavalink4NET.Integrations.TextToSpeech.Synthesis;
using Lavalink4NET.Payloads;

internal sealed class TextToSpeechTrackEncoder
{
    public static Uri EncodeMessage(string message, string? languageName = null)
    {
        var builder = new UriBuilder
        {
            Scheme = "tts",
            Host = Uri.EscapeDataString(message),
        };

        if (languageName is not null)
        {
            var queryParametersBuilder = HttpUtility.ParseQueryString(string.Empty);
            queryParametersBuilder["language"] = languageName;
            builder.Query = queryParametersBuilder.ToString();
        }

        return builder.Uri;
    }

    public static Uri EncodeMessage(SynthesisInput input, VoiceSelectionParameters? voiceSelectionParameters = null, AudioConfiguration? audioConfiguration = null)
    {
        var pooledBufferWriter = new PooledBufferWriter();
        using (var utf8JsonWriter = new Utf8JsonWriter(pooledBufferWriter))
        {
            utf8JsonWriter.WriteStartObject();
            utf8JsonWriter.WritePropertyName("input");
            JsonSerializer.Serialize(utf8JsonWriter, input);

            if (voiceSelectionParameters is not null)
            {
                utf8JsonWriter.WritePropertyName("voice");
                JsonSerializer.Serialize(utf8JsonWriter, voiceSelectionParameters);
            }

            if (audioConfiguration is not null)
            {
                utf8JsonWriter.WritePropertyName("audioConfig");
                JsonSerializer.Serialize(utf8JsonWriter, audioConfiguration);
            }

            utf8JsonWriter.WriteEndObject();
        }

        var configurationString = Encoding.UTF8.GetString(pooledBufferWriter.WrittenSegment);
        return new Uri($"tts://?config={HttpUtility.UrlEncode(configurationString)}");
    }
}
