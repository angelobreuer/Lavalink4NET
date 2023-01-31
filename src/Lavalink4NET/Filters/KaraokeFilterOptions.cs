namespace Lavalink4NET.Filters;

using System.Text.Json.Serialization;


public sealed class KaraokeFilterOptions : IFilterOptions
{
    public const string Name = "karaoke";

    /// <inheritdoc/>
    string IFilterOptions.Name => Name;

    [JsonPropertyName("level")]
    public float Level { get; set; } = 1.0F;

    [JsonPropertyName("monoLevel")]
    public float MonoLevel { get; set; } = 1.0F;

    [JsonPropertyName("filterBand")]
    public float FilterBand { get; set; } = 220.0F;

    [JsonPropertyName("filterWidth")]
    public float FilterWidth { get; set; } = 100.0F;
}
