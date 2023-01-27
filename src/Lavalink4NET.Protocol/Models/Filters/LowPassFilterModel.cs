namespace Lavalink4NET.Protocol.Models;

using System.Text.Json.Serialization;

/// <summary>
///     Initializes a new instance of the <see cref="LowPassFilterModel"/> class.
/// </summary>
/// <remarks>
///     Higher frequencies get suppressed, while lower frequencies pass through this filter, thus the name low pass.
///     Any smoothing values equal to, or less than 1.0 will disable the filter.
///     <seealso href="https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md#channel-mix"/>
/// </remarks>
/// <param name="SmoothingFactor">The smoothing factor (1.0 &lt; x).</param>
public sealed record class LowPassFilterModel(
    [property: JsonPropertyName("smoothing")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? SmoothingFactor = null);
