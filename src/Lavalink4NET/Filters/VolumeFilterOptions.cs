namespace Lavalink4NET.Filters;

using System.Text.Json.Serialization;


[JsonConverter(typeof(VolumeFilterOptionsJsonConverter))]
public sealed class VolumeFilterOptions : IFilterOptions
{
    public const string Name = "volume";

    /// <inheritdoc/>
    string IFilterOptions.Name => Name;

    [JsonPropertyName("volume")]
    public float Volume { get; init; }
}
