namespace Lavalink4NET.Protocol.Models.Filters;

using System.Text.Json.Serialization;

/// <summary>
///     Initializes a new instance of the <see cref="RotationFilterModel"/> class.
/// </summary>
/// <remarks>
///     Rotates the sound around the stereo channels/user headphones aka Audio Panning
///     <seealso href="https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md#rotation"/>
/// </remarks>
/// <param name="Frequency">The frequency of the audio rotating around the listener in Hz.</param>
public sealed record class RotationFilterModel(
    [property: JsonPropertyName("rotationHz")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? Frequency = null);
