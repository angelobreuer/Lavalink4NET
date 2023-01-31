namespace Lavalink4NET.Filters;

using System.Text.Json.Serialization;


public sealed class LowPassFilterOptions : IFilterOptions
{
    public const string Name = "lowPass";

    /// <inheritdoc/>
    string IFilterOptions.Name => Name;

    [JsonPropertyName("smoothing")]
    public float Smoothing { get; set; } = 20.0F;
}
