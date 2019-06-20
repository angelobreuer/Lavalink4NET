namespace Lavalink4NET.Util
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    ///     A Json.Net JSON converter between a milliseconds <see cref="double"/> and a <see cref="TimeSpan"/>.
    /// </summary>
    internal sealed class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        /// <summary>
        ///     Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">
        ///     The existing value of object being read. If there is no existing value then null will
        ///     be used.
        /// </param>
        /// <param name="hasExistingValue">The existing value has a value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
            => TimeSpan.FromMilliseconds(double.Parse(reader.Value.ToString()));

        /// <summary>
        ///     Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
            => writer.WriteValue(value.TotalMilliseconds);
    }
}