namespace Lavalink4NET.Filters
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class VibratoFilterOptions : IFilterOptions
    {
        public const string Name = "vibrato";

        /// <inheritdoc/>
        string IFilterOptions.Name => Name;

        [JsonProperty("frequency")]
        public float Frequency { get; set; } = 2.0F;

        [JsonProperty("depth")]
        public float Depth { get; set; } = 0.5F;
    }
}
