namespace Lavalink4NET.Filters;

using System.Text.Json.Serialization;


public sealed class ChannelMixFilterOptions : IFilterOptions
{
    public const string Name = "channelMix";

    /// <inheritdoc/>
    string IFilterOptions.Name => Name;

    [JsonPropertyName("leftToLeft")]
    public float LeftToLeft { get; init; } = 1F;

    [JsonPropertyName("leftToRight")]
    public float LeftToRight { get; init; } = 0F;

    [JsonPropertyName("rightToLeft")]
    public float RightToLeft { get; init; } = 0F;

    [JsonPropertyName("rightToRight")]
    public float RightToRight { get; init; } = 1F;
}
