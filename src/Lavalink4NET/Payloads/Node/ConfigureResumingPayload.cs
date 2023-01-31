namespace Lavalink4NET.Payloads.Node;

using System.Text.Json.Serialization;

/// <summary>
///     The strongly-typed representation of a configure resuming payload which sent to the
///     lavalink node (in serialized JSON format). For more reference see https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md
/// </summary>
public sealed class ConfigureResumingPayload
{
    /// <summary>
    ///     Gets the resuming key.
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the number of seconds after disconnecting before the session is closed anyways.
    /// </summary>
    [JsonPropertyName("timeout")]
    public int Timeout { get; init; }
}
