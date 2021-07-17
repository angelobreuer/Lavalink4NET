namespace Lavalink4NET.Filters
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class TimescaleFilterOptions : IFilterOptions
    {
        public const string Name = "timescale";

        /// <inheritdoc/>
        string IFilterOptions.Name => Name;

        [JsonProperty("speed")]
        public float Speed { get; set; } = 1.0F;

        [JsonProperty("pitch")]
        public float Pitch { get; set; } = 1.0F;

        [JsonProperty("rate")]
        public float Rate { get; set; } = 1.0F;
    }
}
