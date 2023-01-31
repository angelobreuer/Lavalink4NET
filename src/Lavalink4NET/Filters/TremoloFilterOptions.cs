namespace Lavalink4NET.Filters;

using System.Text.Json.Serialization;


public sealed class TremoloFilterOptions : IFilterOptions
{
    public const string Name = "tremolo";

    /// <inheritdoc/>
    string IFilterOptions.Name => Name;

    [JsonPropertyName("frequency")]
    public float Frequency { get; set; } = 2.0F;

    [JsonPropertyName("depth")]
    public float Depth { get; set; } = 0.5F;
}
