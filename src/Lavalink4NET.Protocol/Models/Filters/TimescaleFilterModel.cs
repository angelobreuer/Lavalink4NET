namespace Lavalink4NET.Protocol.Models.Filters;

using System.Text.Json.Serialization;

/// <summary>
///     Initializes a new instance of the <see cref="TimescaleFilterModel"/> class.
/// </summary>
/// <remarks>
///     Changes the speed, pitch, and rate. All default to 1.0.
///     <seealso href="https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md#timescale"/>
/// </remarks>
/// <param name="Speed">The playback speed (0.0 &lt;= x).</param>
/// <param name="Pitch">The pitch (0.0 &lt;= x).</param>
/// <param name="Rate">The rate (0.0 &lt;= x).</param>
public sealed record class TimescaleFilterModel(
    [property: JsonPropertyName("speed")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? Speed = null,

    [property: JsonPropertyName("pitch")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? Pitch = null,

    [property: JsonPropertyName("rate")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? Rate = null);
