namespace Lavalink4NET.Protocol.Models;

using System.Text.Json.Serialization;

/// <summary>
///     Initializes a new instance of the <see cref="KaraokeFilterModel"/> class.
/// </summary>
/// <remarks>
///     Uses equalization to eliminate part of a band, usually targeting vocals.
///     <seealso href="https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md#karaoke"/>
/// </remarks>
/// <param name="Level">The level (0 to 1.0 where 0.0 is no effect and 1.0 is full effect).</param>
/// <param name="MonoLevel">The mono level (0 to 1.0 where 0.0 is no effect and 1.0 is full effect).</param>
/// <param name="FilterBand">The filter band.</param>
/// <param name="FilterWidth">The filter width.</param>
public sealed record class KaraokeFilterModel(
    [property: JsonPropertyName("level")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? Level = null,

    [property: JsonPropertyName("monoLevel")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? MonoLevel = null,

    [property: JsonPropertyName("filterBand")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? FilterBand = null,

    [property: JsonPropertyName("filterWidth")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? FilterWidth = null);
