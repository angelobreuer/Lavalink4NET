namespace Lavalink4NET.Filters;

using System.Text.Json.Serialization;


public sealed class RotationFilterOptions : IFilterOptions
{
    public const string Name = "rotation";

    /// <inheritdoc/>
    string IFilterOptions.Name => Name;

    [JsonPropertyName("rotationHz")]
    public float Frequency { get; set; } = 0F;
}
