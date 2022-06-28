namespace Lavalink4NET.Integrations.ExtraFilters;

using System.Text.Json.Serialization;
using Lavalink4NET.Filters;

public sealed class EchoFilterOptions : IFilterOptions
{
    public const string Name = "echo";

    /// <inheritdoc/>
    string IFilterOptions.Name => Name;

    [JsonPropertyName("delay")]
    public float Delay { get; init; } = 1.0F;

    [JsonPropertyName("decay")]
    public float Decay { get; init; } = 0.5F;
}
