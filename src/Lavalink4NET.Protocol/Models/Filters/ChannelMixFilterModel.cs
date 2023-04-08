namespace Lavalink4NET.Protocol.Models.Filters;

using System.Text.Json.Serialization;

/// <summary>
///     Initializes a new instance of the <see cref="ChannelMixFilterModel"/> class.
/// </summary>
/// <remarks>
///     Mixes both channels (left and right), with a configurable factor on how much each channel affects the other. 
///     With the defaults, both channels are kept independent of each other. 
///     Setting all factors to 0.5 means both channels get the same audio.
///     <seealso href="https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md#channel-mix"/>
/// </remarks>
/// <param name="LeftToLeft">The left to left channel mix factor (0.0 &lt;= x &lt;= 1.0)</param>
/// <param name="LeftToRight">The left to right channel mix factor (0.0 &lt;= x &lt;= 1.0)</param>
/// <param name="RightToLeft">The right to left channel mix factor (0.0 &lt;= x &lt;= 1.0)</param>
/// <param name="RightToRight">The right to right channel mix factor (0.0 &lt;= x &lt;= 1.0)</param>
public sealed record class ChannelMixFilterModel(
    [property: JsonPropertyName("leftToLeft")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? LeftToLeft = null,

    [property: JsonPropertyName("leftToRight")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? LeftToRight = null,

    [property: JsonPropertyName("rightToLeft")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? RightToLeft = null,

    [property: JsonPropertyName("rightToRight")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? RightToRight = null);
