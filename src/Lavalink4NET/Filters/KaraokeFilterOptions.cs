namespace Lavalink4NET.Filters
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class KaraokeFilterOptions : IFilterOptions
    {
        public const string Name = "karaoke";

        /// <inheritdoc/>
        string IFilterOptions.Name => Name;

        [JsonProperty("level")]
        public float Level { get; set; } = 1.0F;

        [JsonProperty("monoLevel")]
        public float MonoLevel { get; set; } = 1.0F;

        [JsonProperty("filterBand")]
        public float FilterBand { get; set; } = 220.0F;

        [JsonProperty("filterWidth")]
        public float FilterWidth { get; set; } = 100.0F;
    }
}
