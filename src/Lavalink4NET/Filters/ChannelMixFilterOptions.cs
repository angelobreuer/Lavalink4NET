namespace Lavalink4NET.Filters
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class ChannelMixFilterOptions : IFilterOptions
    {
        public const string Name = "channelMix";

        /// <inheritdoc/>
        string IFilterOptions.Name => Name;

        [JsonProperty("leftToLeft")]
        public float LeftToLeft { get; set; } = 1F;

        [JsonProperty("leftToRight")]
        public float LeftToRight { get; set; } = 0F;

        [JsonProperty("rightToLeft")]
        public float RightToLeft { get; set; } = 0F;

        [JsonProperty("rightToRight")]
        public float RightToRight { get; set; } = 1F;
    }
}
