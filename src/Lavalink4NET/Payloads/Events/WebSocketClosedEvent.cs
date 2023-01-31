namespace Lavalink4NET.Payloads.Events;

using System.Net.WebSockets;
using System.Text.Json.Serialization;

/// <summary>
///     The strongly-typed representation of a web socket closed event received from the
///     lavalink node (in serialized JSON format). For more reference see https://github.com/freyacodes/Lavalink/blob/master/IMPLEMENTATION.md
/// </summary>
public sealed class WebSocketClosedEvent
{
    /// <summary>
    ///     Gets the web-socket close code.
    /// </summary>
    [JsonPropertyName("code")]
    public WebSocketCloseStatus CloseCode { get; init; }

    /// <summary>
    ///     Gets the reason why the web-socket was closed.
    /// </summary>
    [JsonPropertyName("reason")]
    public string Reason { get; init; } = string.Empty;

    /// <summary>
    ///     Gets a value indicating whether the connection was closed by the remote (discord gateway).
    /// </summary>
    [JsonPropertyName("byRemote")]
    public bool ByRemote { get; init; }
}
