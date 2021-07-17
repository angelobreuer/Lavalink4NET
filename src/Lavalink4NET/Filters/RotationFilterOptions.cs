namespace Lavalink4NET.Filters
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class RotationFilterOptions : IFilterOptions
    {
        public const string Name = "rotation";

        /// <inheritdoc/>
        string IFilterOptions.Name => Name;

        [JsonProperty("rotationHz")]
        public float Frequency { get; set; } = 0F;
    }
}
