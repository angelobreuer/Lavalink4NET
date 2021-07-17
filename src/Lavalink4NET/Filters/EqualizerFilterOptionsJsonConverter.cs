namespace Lavalink4NET.Filters
{
    using System;
    using Newtonsoft.Json;

    internal sealed class EqualizerFilterOptionsJsonConverter : JsonConverter<EqualizerFilterOptions>
    {
        /// <inheritdoc/>
        public override bool CanRead => false;

        /// <inheritdoc/>
        public override EqualizerFilterOptions ReadJson(JsonReader reader, Type objectType, EqualizerFilterOptions existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, EqualizerFilterOptions value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.Bands);
        }
    }
}
