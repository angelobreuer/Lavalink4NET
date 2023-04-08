namespace Lavalink4NET.Protocol.Models.Filters;

using System.Text.Json.Serialization;

/// <summary>
///     Initializes a new instance of the <see cref="TremoloFilterModel"/> class.
/// </summary>
/// <remarks>
///     Uses amplification to create a shuddering effect, where the volume quickly oscillates.
///     <seealso href="https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md#tremolo"/>
/// </remarks>
/// <param name="Frequency">The frequency (0.0 &lt; x).</param>
/// <param name="Depth">The depth (0.0 &lt; x &lt;= 1.0).</param>
public sealed record class TremoloFilterModel(
    [property: JsonPropertyName("frequency")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? Frequency = null,

    [property: JsonPropertyName("depth")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? Depth = null);
