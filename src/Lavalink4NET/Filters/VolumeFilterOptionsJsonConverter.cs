namespace Lavalink4NET.Filters
{
    using System;
    using Newtonsoft.Json;

    internal sealed class VolumeFilterOptionsJsonConverter : JsonConverter<VolumeFilterOptions>
    {
        /// <inheritdoc/>
        public override VolumeFilterOptions ReadJson(JsonReader reader, Type objectType, VolumeFilterOptions existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return new VolumeFilterOptions() { Volume = (float)reader.Value, };
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, VolumeFilterOptions value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Volume);
        }
    }
}
