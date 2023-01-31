namespace Lavalink4NET.Filters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class VolumeFilterOptionsJsonConverter : JsonConverter<VolumeFilterOptions>
{
    /// <inheritdoc/>
    public override VolumeFilterOptions? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new VolumeFilterOptions { Volume = reader.GetSingle(), };
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, VolumeFilterOptions value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Volume);
    }
}
