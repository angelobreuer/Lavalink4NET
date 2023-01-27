namespace Lavalink4NET.Protocol.Models;
using System.Text.Json.Serialization;

/// <summary>
///     Initializes a new instance of the <see cref="DistortionFilterModel"/> class.
/// </summary>
/// <remarks>
///     <seealso href="https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md#distortion"/>
/// </remarks>
/// <param name="SinOffset">the sinus offset.</param>
/// <param name="SinScale">the sinus scale.</param>
/// <param name="CosOffset">the co-sinus offset.</param>
/// <param name="CosScale">the co-sinus scale.</param>
/// <param name="TanOffset">the tangent offset.</param>
/// <param name="TanScale">the tangent scale.</param>
/// <param name="Offset">the offset.</param>
/// <param name="Scale">the scale.</param>
public sealed record class DistortionFilterModel(
    [property: JsonPropertyName("sinOffset")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? SinOffset = null,

    [property: JsonPropertyName("sinScale")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? SinScale = null,

    [property: JsonPropertyName("cosOffset")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? CosOffset = null,

    [property: JsonPropertyName("cosScale")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? CosScale = null,

    [property: JsonPropertyName("tanOffset")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? TanOffset = null,

    [property: JsonPropertyName("tanScale")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? TanScale = null,

    [property: JsonPropertyName("offset")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? Offset = null,

    [property: JsonPropertyName("scale")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? Scale = null);
