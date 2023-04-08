namespace Lavalink4NET.Integrations.ExtraFilters;

using System.Text.Json.Serialization;

public sealed record class EchoFilterModel(
    [property: JsonPropertyName("delay")]
    float? Delay = null,

    [property: JsonPropertyName("decay")]
    float?Decay = null);