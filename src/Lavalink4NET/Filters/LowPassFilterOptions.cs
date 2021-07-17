namespace Lavalink4NET.Filters
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class LowPassFilterOptions : IFilterOptions
    {
        public const string Name = "lowPass";

        /// <inheritdoc/>
        string IFilterOptions.Name => Name;

        [JsonProperty("smoothing")]
        public float Smoothing { get; set; } = 20.0F;
    }
}
