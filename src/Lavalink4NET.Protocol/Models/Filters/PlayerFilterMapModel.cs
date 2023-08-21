namespace Lavalink4NET.Protocol.Models.Filters;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
///     Initializes a new instance of the <see cref="PlayerFilterMapModel"/> class.
/// </summary>
/// <param name="Volume">Lets you adjust the player volume from 0.0 to 5.0 where 1.0 is 100%. Values &gt; 1.0 may cause clipping</param>
/// <param name="Equalizer">Lets you adjust 15 different bands.</param>
/// <param name="Karaoke">Lets you eliminate part of a band, usually targeting vocals.</param>
/// <param name="Timescale">Lets you change the speed, pitch, and rate.</param>
/// <param name="Tremolo">Lets you create a shuddering effect, where the volume quickly oscillates.</param>
/// <param name="Vibrato">Lets you create a shuddering effect, where the pitch quickly oscillates.</param>
/// <param name="Rotation">Lets you rotate the sound around the stereo channels/user headphones aka Audio Panning.</param>
/// <param name="Distortion">Lets you distort the audio.</param>
/// <param name="ChannelMix">Lets you mix both channels (left and right).</param>
/// <param name="LowPass">Lets you filter higher frequencies.</param>
public sealed record class PlayerFilterMapModel(
    [property: JsonPropertyName("volume")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    float? Volume = null,

    [property: JsonPropertyName("equalizer")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    ImmutableArray<EqualizerBandModel>? Equalizer = null,

    [property: JsonPropertyName("karaoke")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    KaraokeFilterModel? Karaoke = null,

    [property: JsonPropertyName("timescale")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    TimescaleFilterModel? Timescale = null,

    [property: JsonPropertyName("tremolo")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    TremoloFilterModel? Tremolo = null,

    [property: JsonPropertyName("vibrato")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    VibratoFilterModel? Vibrato = null,

    [property: JsonPropertyName("rotation")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    RotationFilterModel? Rotation = null,

    [property: JsonPropertyName("distortion")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    DistortionFilterModel? Distortion = null,

    [property: JsonPropertyName("channelMix")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    ChannelMixFilterModel? ChannelMix = null,

    [property: JsonPropertyName("lowPass")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    LowPassFilterModel? LowPass = null)
{
    [property: JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalFilters { get; set; }
}
