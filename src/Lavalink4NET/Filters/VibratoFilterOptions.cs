namespace Lavalink4NET.Filters;

using System.Text.Json.Serialization;


public sealed class VibratoFilterOptions : IFilterOptions
{
    public const string Name = "vibrato";

    /// <inheritdoc/>
    string IFilterOptions.Name => Name;

    [JsonPropertyName("frequency")]
    public float Frequency { get; set; } = 2.0F;

    [JsonPropertyName("depth")]
    public float Depth { get; set; } = 0.5F;
}
