namespace Lavalink4NET.Protocol.Models.Filters;

using System.Text.Json.Serialization;

/// <summary>
///     Initializes a new instance of the <see cref="VibratoFilterModel"/> class.
/// </summary>
/// <remarks>
///     Similar to tremolo. While tremolo oscillates the volume, vibrato oscillates the pitch.
///     <seealso href="https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md#vibrato"/>
/// </remarks>
/// <param name="Frequency">The frequency (0.0 &lt; x &lt;= 14.0).</param>
/// <param name="Depth">The depth (0.0 &lt; x &lt;= 1.0).</param>
public sealed record class VibratoFilterModel(
    [property: JsonPropertyName("frequency")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? Frequency = null,

    [property: JsonPropertyName("depth")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? Depth = null);
