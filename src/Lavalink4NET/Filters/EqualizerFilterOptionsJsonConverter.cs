namespace Lavalink4NET.Filters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class EqualizerFilterOptionsJsonConverter : JsonConverter<EqualizerFilterOptions>
{
    /// <inheritdoc/>
    public override EqualizerFilterOptions? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, EqualizerFilterOptions value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Bands, options);
    }
}
