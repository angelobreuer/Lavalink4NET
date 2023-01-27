namespace Lavalink4NET.Protocol.Models;

using System.Text.Json.Serialization;

/// <summary>
///     Initializes a new instance of the <see cref="EqualizerBandModel"/> class.
/// </summary>
/// <param name="Band">
///     the equalizer band (0 to 14).
///     
///     <list type="number">
///         <item>
///             <term>Band 0</term>
///             <description>25 Hz</description>
///         </item>
///         <item>
///             <term>Band 1</term>
///             <description>40 Hz</description>
///         </item>
///         <item>
///             <term>Band 2</term>
///             <description>63 Hz</description>
///         </item>
///         <item>
///             <term>Band 3</term>
///             <description>100 Hz</description>
///         </item>
///         <item>
///             <term>Band 4</term>
///             <description>160 Hz</description>
///         </item>
///         <item>
///             <term>Band 5</term>
///             <description>250 Hz</description>
///         </item>
///         <item>
///             <term>Band 6</term>
///             <description>400 Hz</description>
///         </item>
///         <item>
///             <term>Band 7</term>
///             <description>630 Hz</description>
///         </item>
///         <item>
///             <term>Band 8</term>
///             <description>1 kHz</description>
///         </item>
///         <item>
///             <term>Band 9</term>
///             <description>1.6 kHz</description>
///         </item>
///         <item>
///             <term>Band 10</term>
///             <description>2.5 kHz</description>
///         </item>
///         <item>
///             <term>Band 11</term>
///             <description>4 kHz</description>
///         </item>
///         <item>
///             <term>Band 12</term>
///             <description>6.3 kHz</description>
///         </item>
///         <item>
///             <term>Band 13</term>
///             <description>10 kHz</description>
///         </item>
///         <item>
///             <term>Band 14</term>
///             <description>16 kHz</description>
///         </item>
///     </list>
/// </param>
/// <param name="Gain">the gain (-0.25 to 1.0)</param>
public sealed record class EqualizerBandModel(
    [property: JsonPropertyName("band")]
    int Band,// TODO: there is probably a typo in the Lavalink docs

    [property: JsonPropertyName("gain")]
    float Gain);