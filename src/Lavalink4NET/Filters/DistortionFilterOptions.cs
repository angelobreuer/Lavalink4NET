namespace Lavalink4NET.Filters;

using System.Text.Json.Serialization;


public sealed class DistortionFilterOptions : IFilterOptions
{
    public const string Name = "distortion";

    /// <inheritdoc/>
    string IFilterOptions.Name => Name;

    [JsonPropertyName("sinOffset")]
    public float SinOffset { get; set; } = 0F;

    [JsonPropertyName("sinScale")]
    public float SinScale { get; set; } = 1F;

    [JsonPropertyName("cosOffset")]
    public float CosOffset { get; set; } = 0F;

    [JsonPropertyName("cosScale")]
    public float CosScale { get; set; } = 1F;

    [JsonPropertyName("tanOffset")]
    public float TanOffset { get; set; } = 0F;

    [JsonPropertyName("tanScale")]
    public float TanScale { get; set; } = 1F;

    [JsonPropertyName("offset")]
    public float Offset { get; set; } = 0F;

    [JsonPropertyName("scale")]
    public float Scale { get; set; } = 1F;
}
