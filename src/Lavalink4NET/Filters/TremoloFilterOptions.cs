namespace Lavalink4NET.Filters
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class TremoloFilterOptions : IFilterOptions
    {
        public const string Name = "tremolo";

        /// <inheritdoc/>
        string IFilterOptions.Name => Name;

        [JsonProperty("frequency")]
        public float Frequency { get; set; } = 2.0F;

        [JsonProperty("depth")]
        public float Depth { get; set; } = 0.5F;
    }
}
