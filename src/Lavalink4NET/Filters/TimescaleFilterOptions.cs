namespace Lavalink4NET.Filters;

using System.Text.Json.Serialization;


public sealed class TimescaleFilterOptions : IFilterOptions
{
    public const string Name = "timescale";

    /// <inheritdoc/>
    string IFilterOptions.Name => Name;

    [JsonPropertyName("speed")]
    public float Speed { get; set; } = 1.0F;

    [JsonPropertyName("pitch")]
    public float Pitch { get; set; } = 1.0F;

    [JsonPropertyName("rate")]
    public float Rate { get; set; } = 1.0F;
}
