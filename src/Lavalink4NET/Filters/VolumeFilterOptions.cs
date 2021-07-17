namespace Lavalink4NET.Filters
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [JsonConverter(typeof(VolumeFilterOptionsJsonConverter))]
    public sealed class VolumeFilterOptions : IFilterOptions
    {
        public const string Name = "volume";

        /// <inheritdoc/>
        string IFilterOptions.Name => Name;

        [JsonProperty("volume")]
        public float Volume { get; set; }
    }
}
