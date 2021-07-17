namespace Lavalink4NET.Filters
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class DistortionFilterOptions : IFilterOptions
    {
        public const string Name = "distortion";

        /// <inheritdoc/>
        string IFilterOptions.Name => Name;

        [JsonProperty("sinOffset")]
        public float SinOffset { get; set; } = 0F;

        [JsonProperty("sinScale")]
        public float SinScale { get; set; } = 1F;

        [JsonProperty("cosOffset")]
        public float CosOffset { get; set; } = 0F;

        [JsonProperty("cosScale")]
        public float CosScale { get; set; } = 1F;

        [JsonProperty("tanOffset")]
        public float TanOffset { get; set; } = 0F;

        [JsonProperty("tanScale")]
        public float TanScale { get; set; } = 1F;

        [JsonProperty("offset")]
        public float Offset { get; set; } = 0F;

        [JsonProperty("scale")]
        public float Scale { get; set; } = 1F;
    }
}
