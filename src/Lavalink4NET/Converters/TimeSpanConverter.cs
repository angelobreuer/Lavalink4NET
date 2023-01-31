namespace Lavalink4NET.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
///     A Json.Net JSON converter between a milliseconds <see cref="double"/> and a <see cref="TimeSpan"/>.
/// </summary>
internal sealed class TimeSpanJsonConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.Number or JsonTokenType.String)
        {
            throw new JsonException();
        }

        if (!reader.TryGetInt64(out var value) &&
            !long.TryParse(reader.GetString(), out value))
        {
            return TimeSpan.Zero;
        }

        if (value < 0)
        {
            return TimeSpan.Zero;
        }

        // infinite time
        if (value is long.MaxValue)
        {
            return TimeSpan.MaxValue;
        }

        return TimeSpan.FromMilliseconds(value);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.TotalMilliseconds);
    }
}
